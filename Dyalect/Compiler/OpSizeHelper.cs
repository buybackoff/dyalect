﻿namespace Dyalect.Compiler
{
    internal static class OpSizeHelper
    {
        internal static int[] Op =
        {
            0, //Nop
            0, //Str
            0, //This
            0, //Pop
            0, //PushNil
            0, //PushI1_0
            0, //PushI1_1
            1, //PushI8
            0, //PushI8_0
            0, //PushI8_1
            1, //PushR8
            0, //PushR8_0
            0, //PushR8_1
            1, //PushStr
            1, //PushCh
            1, //Br
            1, //Brtrue
            1, //Brfalse
            0, //Shl
            0, //Shr
            0, //And
            0, //Or
            0, //Xor
            0, //Add
            0, //Sub
            0, //Mul
            0, //Div
            0, //Rem
            0, //Neg
            0, //Plus
            0, //Not
            0, //BitNot
            0, //Len
            0, //Gt
            0, //Lt
            0, //Eq
            0, //NotEq
            0, //GtEq
            0, //LtEq
            1, //Pushloc
            1, //Pushvar
            1, //Pushext
            1, //Poploc
            1, //Popvar
            0, //Ret
            0, //Dup
            0, //SyncPoint
            0, //Fail
            1, //FailSys
            1, //NewFun
            1, //NewFunV
            1, //NewIter
            1, //SetMember
            1, //SetMemberT
            1, //GetMember
            1, //HasMember
            1, //SetMemberS
            1, //SetMemberST
            0, //Get
            0, //Set
            1, //RunMod
            0, //Type
            1, //Tag
            0, //Term
            0, //Yield
            0, //PushNilT
            1, //Brterm
            1, //Briter
            1, //Aux

            1, //FunPrep
            1, //FunArgIx
            1, //FunArgNm
            1, //FunCall
            1, //NewTuple
            1, //GetIx
            1, //SetIx
            1, //HasField
            1, //TypeCheck
            1, //TypeCheckT
            1, //Start
            0, //End
            1, //NewType
            1, //TypeS
            1, //TypeST
            1, //CtorCheck
            0, //Unbox
        };
    }
}
