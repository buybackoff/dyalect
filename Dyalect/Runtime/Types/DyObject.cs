﻿using Dyalect.Compiler;
using System;

namespace Dyalect.Runtime.Types
{
    public abstract class DyObject : IEquatable<DyObject>
    {
        internal readonly int TypeId;

        internal DyObject(int typeId)
        {
            TypeId = typeId;
        }

        public override string ToString() => AsString();

        public virtual bool AsBool() => true;

        public virtual long AsInteger() => 0L;

        public virtual double AsFloat() => .0d;

        public virtual string AsString() => "";

        public abstract object AsObject();

        protected abstract bool TestEquality(DyObject obj);
        public bool Equals(DyObject obj)
        {
            if (TypeId != obj.TypeId)
                return false;
            return TestEquality(obj);
        }

        internal protected virtual DyObject GetItem(DyObject index, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.GetName, this.TypeName(ctx)).Set(ctx);

        internal protected virtual void SetItem(DyObject index, DyObject value, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.SetName, this.TypeName(ctx)).Set(ctx);
    }

    internal static class DyObjectInternalExtensions
    {
        public static string TypeName(this DyObject self, ExecutionContext ctx) => ctx.Assembly.Types[self.TypeId].TypeName;

        public static DyTypeInfo Type(this DyObject self, ExecutionContext ctx) => ctx.Assembly.Types[self.TypeId];

        public static DyString ToString(this DyObject self, ExecutionContext ctx) => ctx.Assembly.Types[self.TypeId].ToString(self, ctx);
    }

    public static class DyObjectExtensions
    {
        public static string Format(this DyObject self, ExecutionContext ctx) => ctx.Assembly.Types[self.TypeId].ToString(self, ctx).AsString();
    }
}
