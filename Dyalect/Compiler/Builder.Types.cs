﻿using Dyalect.Parser;
using Dyalect.Parser.Model;
using static Dyalect.Compiler.Hints;

namespace Dyalect.Compiler
{
    //This part is responsible for the compilation logic of custom types
    partial class Builder
    {
        private void Build(DTypeDeclaration node, Hints hints, CompilerContext ctx)
        {
            var typeId = unit.Types.Count;
            var unitId = unit.UnitIds.Count - 1;
            var ti = new TypeInfo(typeId, new UnitInfo(unitId, unit));

            if (types.ContainsKey(node.Name))
            {
                types.Remove(node.Name);
                AddError(CompilerError.TypeAlreadyDeclared, node.Location, node.Name);
            }

            types.Add(node.Name, ti);

            var td = new TypeDescriptor(node.Name, typeId, node.HasConstructors);
            unit.Types.Add(td);
            unit.TypeMap.Add(node.Name, td);

            if (node.HasConstructors)
            {
                var nh = hints.Remove(Push).Remove(ExpectPush);

                foreach (var c in node.Constructors)
                    Build(c, nh, ctx);
            }

            PushIf(hints);
        }

        private void GenerateConstructor(DFunctionDeclaration func, Hints hints, CompilerContext ctx)
        {
            if (func.Parameters.Count == 0)
            {
                AddLinePragma(func);
                cw.PushNil();
            }
            else if (func.Parameters.Count == 1)
            {
                var p = func.Parameters[0];
                var a = GetVariable(p.Name, p);
                AddLinePragma(func);
                cw.PushVar(a);
                cw.Tag(p.Name);
            }
            else
            {
                for (var i = 0; i < func.Parameters.Count; i++)
                {
                    var p = func.Parameters[i];
                    var a = GetVariable(p.Name, p);
                    cw.PushVar(a);
                    cw.Tag(p.Name);
                }

                AddLinePragma(func);
                cw.NewTuple(func.Parameters.Count);
            }

            TryGetLocalType(func.TypeName.Local, out var ti);
            cw.Aux(GetMemberNameId(func.Name));
            cw.NewType(ti.TypeId);
        }

        private TypeHandle GetTypeHandle(Qualident name, Location loc) => GetTypeHandle(name.Parent, name.Local, loc);

        private TypeHandle GetTypeHandle(string parent, string local, Location loc)
        {
            var err = GetTypeHandle(parent, local, out var handle, out var std);

            if (err == CompilerError.UndefinedModule)
                AddError(err, loc, parent);
            else if (err == CompilerError.UndefinedType)
                AddError(err, loc, local);

            return new TypeHandle(handle, std);
        }

        private bool IsTypeExists(string name)
        {
            var err = GetTypeHandle(null, name, out var _, out var _);
            return err == CompilerError.None;
        }

        private CompilerError GetTypeHandle(string parent, string local, out int handle, out bool std)
        {
            handle = -1;
            std = false;

            if (parent == null)
                handle = DyType.GetTypeCodeByName(local);

            if (handle > -1)
            {
                std = true;
                return CompilerError.None;
            }

            if (parent == null)
            {
                if (!TryGetType(local, out var ti))
                    return CompilerError.UndefinedType;
                else
                {
                    handle = (byte)ti.Unit.Handle | ti.TypeId << 8;
                    return CompilerError.None;
                }
            }
            else
            {
                if (!referencedUnits.TryGetValue(parent, out var ui))
                    return CompilerError.UndefinedModule;
                else
                {
                    if (!TryGetExternalType(ui, local, out var id))
                        return CompilerError.UndefinedType;

                    handle = (byte)ui.Handle | id << 8;
                    return CompilerError.None;
                }
            }
        }

        private bool TryGetType(string local, out TypeInfo ti)
        {
            if (TryGetLocalType(local, out ti))
                return true;

            foreach (var r in referencedUnits.Values)
                if (TryGetExternalType(r, local, out var id))
                {
                    ti = new TypeInfo(id, r);
                    return true;
                }

            return false;
        }

        private bool TryGetExternalType(UnitInfo ui, string typeName, out int id)
        {
            id = -1;

            if (ui.Unit.TypeMap.TryGetValue(typeName, out var td))
            {
                id = td.Id;
                return true;
            }

            return false;
        }

        private bool TryGetLocalType(string typeName, out TypeInfo ti)
        {
            return types.TryGetValue(typeName, out ti);
        }
    }
}
