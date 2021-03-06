﻿using System.Text;

namespace Dyalect.Parser.Model
{
    public sealed class DIf : DNode
    {
        public DIf(Location loc) : base(NodeType.If, loc)
        {

        }

        public DNode Condition { get; set; }

        public DNode True { get; set; }

        public DNode False { get; set; }

        internal override void ToString(StringBuilder sb)
        {
            sb.Append("if ");
            Condition.ToString(sb);
            True.ToString(sb);

            if (False != null)
            {
                sb.Append("else ");
                False.ToString(sb);
            }
        }
    }
}
