﻿using Dyalect.Compiler;
using Dyalect.Parser;
using System;
using System.Collections.Generic;

namespace Dyalect.Runtime.Types
{
    public sealed class DyString : DyObject
    {
        public static readonly DyString Empty = new DyString("");
        internal readonly string Value;

        public DyString(string str) : base(StandardType.String)
        {
            Value = str;
        }

        public override object ToObject() => Value;

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is DyString s)
                return Value.Equals(s.Value);
            else
                return false;
        }

        protected override bool TestEquality(DyObject obj) => Value == obj.GetString();

        internal protected override string GetString() => Value;

        public static implicit operator string(DyString str) => str.Value;

        public static implicit operator DyString(string str) => new DyString(str);

        protected internal override DyObject GetItem(DyObject index, ExecutionContext ctx)
        {
            if (index.TypeId != StandardType.Integer)
                return Err.IndexInvalidType(this.TypeName(ctx), index.TypeName(ctx)).Set(ctx);

            var idx = (int)index.GetInteger();

            if (idx < 0 || idx >= Value.Length)
                return Err.IndexOutOfRange(this.TypeName(ctx), idx).Set(ctx);

            return new DyString(Value[idx].ToString());
        }
    }

    internal sealed class DyStringTypeInfo : DyTypeInfo
    {
        public static readonly DyStringTypeInfo Instance = new DyStringTypeInfo();

        private DyStringTypeInfo() : base(StandardType.Bool)
        {

        }

        public override string TypeName => StandardType.StringName;

        public override DyObject Create(ExecutionContext ctx, params DyObject[] args) =>
            new DyString(args.TakeOne(DyString.Empty).ToString(ctx));

        #region Operations
        protected override DyObject AddOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            var str1 = left.TypeId == StandardType.String ? left.GetString() : left.ToString(ctx).Value;
            var str2 = right.TypeId == StandardType.String ? right.GetString() : right.ToString(ctx).Value;
            return new DyString(str1 + str2);
        }

        protected override DyObject EqOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (left.TypeId == right.TypeId)
                return left.GetString() == right.GetString() ? DyBool.True : DyBool.False;
            else
                return base.EqOp(left, right, ctx);
        }

        protected override DyObject NeqOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (left.TypeId == right.TypeId)
                return left.GetString() != right.GetString() ? DyBool.True : DyBool.False;
            else
                return base.NeqOp(left, right, ctx);
        }

        protected override DyObject GtOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (left.TypeId == right.TypeId)
                return left.GetString().CompareTo(right.GetString()) > 0 ? DyBool.True : DyBool.False;
            else
                return base.GtOp(left, right, ctx);
        }

        protected override DyObject LtOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (left.TypeId == right.TypeId)
                return left.GetString().CompareTo(right.GetString()) < 0 ? DyBool.True : DyBool.False;
            else
                return base.LtOp(left, right, ctx);
        }

        protected override DyObject LengthOp(DyObject arg, ExecutionContext ctx)
        {
            var len = arg.GetString().Length;
            return len == 0 ? DyInteger.Zero
                : len == 1 ? DyInteger.One
                : len == 2 ? DyInteger.Two
                : new DyInteger(len);
        }

        protected override DyString ToStringOp(DyObject arg, ExecutionContext ctx) => StringUtil.Escape(arg.GetString());

        private DyObject GetIterator(DyObject obj, ExecutionContext ctx) => new DyStringIterator((DyString)obj);

        protected override DyFunction GetTrait(string name, ExecutionContext ctx)
        {
            if (name == "iterator")
                return new DyMemberFunction(name, GetIterator);

            return null;
        }
        #endregion
    }

    internal sealed class DyStringIterator : DyObject
    {
        internal DyString String { get; }

        internal int Index { get; set; } = -1;

        public DyStringIterator(DyString str) : base(StandardType.StringIterator)
        {
            String = str;
        }

        public override object ToObject() => ((IEnumerable<object>)String.ToObject()).GetEnumerator();

        protected override bool TestEquality(DyObject obj) => String.Equals(((DyStringIterator)obj).String);
    }

    internal sealed class DyStringIteratorTypeInfo : DyTypeInfo
    {
        public static readonly DyTypeInfo Instance = new DyStringIteratorTypeInfo();

        public DyStringIteratorTypeInfo() : base(StandardType.StringIterator)
        {

        }

        protected override DyObject MoveNextOp(DyObject obj, ExecutionContext ctx)
        {
            var iter = (DyStringIterator)obj;
            iter.Index++;
            return iter.Index < iter.String.Value.Length ? DyBool.True : DyBool.False;
        }

        protected override DyObject GetCurrentOp(DyObject obj, ExecutionContext ctx)
        {
            var iter = (DyStringIterator)obj;
            return new DyString(iter.String.Value[iter.Index].ToString());
        }

        protected override DyFunction GetTrait(string name, ExecutionContext ctx)
        {
            if (name == Traits.NextName)
                return new DyMemberFunction(name, MoveNextOp);

            if (name == Traits.CurName)
                return new DyMemberFunction(name, GetCurrentOp);

            return null;
        }

        public override string TypeName => StandardType.StringIteratorName;

        public override DyObject Create(ExecutionContext ctx, params DyObject[] args) =>
            throw new NotImplementedException();
    }
}
