﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Light.Cron
{
    abstract class CrontabValueNode
    {
        public abstract bool Check(int value);
    }


    class SingleCrontabValueNode : CrontabValueNode
    {
        private readonly int value;

        public SingleCrontabValueNode(int value)
        {
            this.value = value;
        }

        public override bool Check(int value)
        {
            return this.value == value;
        }
    }

    class RangeCrontabValueNode : CrontabValueNode
    {
        private readonly int min;
        private readonly int max;
        private readonly int start;
        private readonly int end;
        private readonly int interval;

        public RangeCrontabValueNode(int min, int max, int start, int end, int interval)
        {
            this.min = min;
            this.max = max;
            this.start = start;
            this.end = end;
            this.interval = interval;
        }

        public override bool Check(int value)
        {
            if (start < end) {
                return value >= start && value <= end && (value - start) % interval == 0;
            }
            else if (start > end) {
                if (value >= start && value <= max && (value - start) % interval == 0) {
                    return true;
                }
                else if (value >= min && value <= end && (value + max - min + 1 - start) % interval == 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return start == value;
            }
        }
    }

    abstract class CrontabDynamicEndValueNode
    {
        public abstract bool Check(int dynamicMax, int value);
    }

    class SingleCrontabDynamicValueNode : CrontabDynamicEndValueNode
    {
        private readonly int value;

        public SingleCrontabDynamicValueNode(int value)
        {
            this.value = value;
        }

        public override bool Check(int dynamicMax, int value)
        {
            var newvalue = dynamicMax + this.value;
            return newvalue == value;
        }
    }

    class RangeCrontabDynamicValueNode : CrontabDynamicEndValueNode
    {
        private readonly int min;
        private readonly int max;
        private readonly int start;
        private readonly int end;
        private readonly int interval;

        public RangeCrontabDynamicValueNode(int min, int max, int start, int end, int interval)
        {
            this.min = min;
            this.max = max;
            this.start = start;
            this.end = end;
            this.interval = interval;
        }

        public override bool Check(int dynamicMax, int value)
        {
            var minN = this.min;
            var maxN = dynamicMax;
            var startN = start > 0 ? start : dynamicMax + start;
            var endN = end > 0 ? end : dynamicMax + end;
            if (startN < min || endN < min) {
                return false;
            }

            if (startN < endN) {
                return value >= startN && value <= endN && (value - startN) % interval == 0;
            }
            else if (startN > endN) {
                if (value >= startN && value <= maxN && (value - startN) % interval == 0) {
                    return true;
                }
                else if (value >= minN && value <= endN && (value + maxN - minN + 1 - startN) % interval == 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return startN == value;
            }
        }
    }
}
