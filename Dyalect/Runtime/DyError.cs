﻿using Dyalect.Debug;
using Dyalect.Runtime.Types;
using Dyalect.Strings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Dyalect.Runtime
{
    public enum DyErrorCode
    {
        None,

        UserCode = 601,

        NotFunction = 602,

        ExternalFunctionFailure = 603,

        OperationNotSupported = 604,

        IndexOutOfRange = 605,

        IndexInvalidType = 606,

        DivideByZero = 607,

        TooManyArguments = 608,

        InvalidType = 609,

        StaticOperationNotSupported = 610,

        AssertFailed = 611,

        RequiredArgumentMissing = 612,

        ArgumentNotFound = 613,

        FailedReadLiteral = 614,

        MatchFailed = 615,

        CollectionModified = 616,

        PrivateNameAccess = 617,

        KeyNotFound = 618,

        KeyAlreadyPresent = 619
    }

    public class DyError
    {
        internal DyError(DyErrorCode code, params (string, object)[] dataItems)
        {
            Code = code;
            DataItems = dataItems;
        }

        internal Stack<StackPoint> Dump { get; set; }

        public DyErrorCode Code { get; }

        public (string Key, object Value)[] DataItems { get; }

        public virtual string GetDescription()
        {
            var key = Code.ToString();

            var sb = new StringBuilder(RuntimeErrors.ResourceManager.GetString(key));

            if (DataItems != null)
                foreach (var dt in DataItems)
                    sb.Replace("%" + dt.Key + "%", (dt.Value ?? "N/A").ToString());

            return sb.ToString();
        }

        internal virtual DyObject GetDyObject()
        {
            return new DyString(GetDescription());
        }
    }

    internal sealed class DyUserError : DyError
    {
        private readonly string dsc;

        public DyUserError(DyObject data, string dsc) : base(DyErrorCode.UserCode)
        {
            Data = data;
            this.dsc = dsc;
        }

        public DyObject Data { get; }

        public override string GetDescription() => dsc;

        internal override DyObject GetDyObject() => Data ?? DyNil.Instance;
    }

    internal static class ExecutionContextExtensions
    {
        public static DyObject CollectionModified(this ExecutionContext ctx)
        {
            ctx.Error = new DyError(DyErrorCode.CollectionModified);
            return DyNil.Instance;
        }

        public static DyObject FailedReadLiteral(this ExecutionContext ctx, string reason)
        {
            ctx.Error = new DyError(DyErrorCode.FailedReadLiteral,
                ("Reason", reason));
            return DyNil.Instance;
        }

        public static DyObject AssertFailed(this ExecutionContext ctx, string reason)
        {
            ctx.Error = new DyError(DyErrorCode.AssertFailed,
                ("Reason", reason));
            return DyNil.Instance;
        }

        public static DyObject StaticOperationNotSupported(this ExecutionContext ctx, string op, string typeName)
        {
            ctx.Error = new DyError(DyErrorCode.StaticOperationNotSupported,
                ("Operation", op),
                ("TypeName", typeName));
            return DyNil.Instance;
        }

        public static DyObject PrivateNameAccess(this ExecutionContext ctx, DyObject obj)
        {
            ctx.Error = new DyError(DyErrorCode.PrivateNameAccess,
                ("Name", obj.ToString()));
            return DyNil.Instance;
        }

        public static DyObject OperationNotSupported(this ExecutionContext ctx, string op, DyObject obj)
        {
            ctx.Error = new DyError(DyErrorCode.OperationNotSupported,
                ("Operation", op),
                ("TypeName", obj.TypeName(ctx)));
            return DyNil.Instance;
        }

        public static DyObject IndexOutOfRange(this ExecutionContext ctx, object index)
        {
            ctx.Error = new DyError(DyErrorCode.IndexOutOfRange,
                ("Index", index));
            return DyNil.Instance;
        }

        public static DyObject IndexInvalidType(this ExecutionContext ctx, DyObject index)
        {
            ctx.Error = new DyError(DyErrorCode.IndexInvalidType,
                ("Index", index),
                ("IndexTypeName", index.TypeName(ctx)));
            return DyNil.Instance;
        }

        public static DyObject KeyNotFound(this ExecutionContext ctx, DyObject key)
        {
            ctx.Error = new DyError(DyErrorCode.KeyNotFound, ("Key", key));
            return DyNil.Instance;
        }

        public static DyObject KeyAlreadyPresent(this ExecutionContext ctx, DyObject key)
        {
            ctx.Error = new DyError(DyErrorCode.KeyAlreadyPresent, ("Key", key));
            return DyNil.Instance;
        }

        public static DyObject InvalidType(this ExecutionContext ctx, DyObject value)
        {
            ctx.Error = new DyError(DyErrorCode.InvalidType,
                ("TypeName", value.TypeName(ctx)));
            return DyNil.Instance;
        }

        public static DyObject ExternalFunctionFailure(this ExecutionContext ctx, string functionName, string error)
        {
            ctx.Error = new DyError(DyErrorCode.ExternalFunctionFailure,
                ("FunctionName", functionName),
                ("Error", error));
            return DyNil.Instance;
        }

        public static DyObject NotFunction(this ExecutionContext ctx, DyObject obj)
        {
            ctx.Error = new DyError(DyErrorCode.NotFunction, ("TypeName", obj.TypeName(ctx)));
            return DyNil.Instance;
        }

        public static DyObject DivideByZero(this ExecutionContext ctx)
        {
            ctx.Error = new DyError(DyErrorCode.DivideByZero);
            return DyNil.Instance;
        }

        public static DyObject TooManyArguments(this ExecutionContext ctx, string functionName, int functionArguments, int passedArguments)
        {
            ctx.Error = new DyError(DyErrorCode.TooManyArguments,
                ("FunctionName", functionName),
                ("FunctionArguments", functionArguments),
                ("PassedArguments", passedArguments));
            return DyNil.Instance;
        }

        public static DyObject RequiredArgumentMissing(this ExecutionContext ctx, string functionName, string argumentName)
        {
            ctx.Error = new DyError(DyErrorCode.RequiredArgumentMissing,
                ("FunctionName", functionName),
                ("ArgumentName", argumentName));
            return DyNil.Instance;
        }

        public static DyObject ArgumentNotFound(this ExecutionContext ctx, string functionName, string argumentName)
        {
            ctx.Error = new DyError(DyErrorCode.ArgumentNotFound,
                ("FunctionName", functionName),
                ("ArgumentName", argumentName));
            return DyNil.Instance;
        }
    }
}
