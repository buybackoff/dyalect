﻿using System;

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

        internal virtual DyObject GetItem(int index) => null;

        internal virtual DyObject GetItem(string key) => null;

        internal virtual bool SetItem(int index, DyObject value) => false;

        internal virtual bool SetItem(string key, DyObject value) => false;
    }

    internal static class DyObjectExtensions
    {
        public static string TypeName(this DyObject self, ExecutionContext ctx) => ctx.Assembly.Types[self.TypeId].TypeName;
    }
}