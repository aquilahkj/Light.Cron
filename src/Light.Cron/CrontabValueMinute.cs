﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    class CrontabValueMinute
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 0;
        const int max = 59;
        readonly List<CrontabValueNode> list;

        public static bool TryParse(string value, out CrontabValueMinute crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new CrontabValueMinute(null);
                    return true;
                }
                if (int.TryParse(item, out int data)) {
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                return false;
            }
            if (list.Count == 0) {
                return false;
            }
            else {
                crontabValue = new CrontabValueMinute(list);
                return true;
            }
        }

        CrontabValueMinute(List<CrontabValueNode> list)
        {
            this.list = list;
        }

        // public override bool AllowNext => false;

        public bool Check(DateTime time, out bool next)
        {
            if (list == null) {
                next = false;
                return true;
            }
            else {
                var value = time.Minute;
                foreach (var item in list) {
                    if (item.Check(value, out next)) {
                        return true;
                    }
                }
                next = false;
                return false;
            }
        }
    }
}
