﻿using Dyalect.Compiler;
using Dyalect.Parser.Model;
using Dyalect.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dyalect.Linker
{
    public sealed class DyIncrementalLinker : DyLinker
    {
        private DyCompiler compiler;
        private UnitComposition composition;
        private int? startOffset;

        private Dictionary<Reference, Unit> backupUnitMap;
        private Dictionary<string, Dictionary<string, Type>> backupAssemblyMap;
        private List<Unit> backupUnits;

        public DyIncrementalLinker(FileLookup lookup, BuilderOptions options) : base(lookup, options)
        {

        }

        public DyIncrementalLinker(FileLookup lookup, BuilderOptions options, DyTuple args) : base(lookup, options, args)
        {

        }

        protected override void Prepare()
        {
            backupUnitMap = new Dictionary<Reference, Unit>(UnitMap);
            backupAssemblyMap = new Dictionary<string, Dictionary<string, Type>>(AssemblyMap);
            backupUnits = new List<Unit>(Units);
        }

        protected override void Complete(bool failed)
        {
            if (failed)
            {
                UnitMap = backupUnitMap;
                AssemblyMap = backupAssemblyMap;
                Units.Clear();

                for (var i = 0; i < backupUnits.Count; i++)
                    Units.Add(backupUnits[i]);
            }
        }

        protected override Result<UnitComposition> Make(Unit unit)
        {
            if (composition == null)
                composition = new UnitComposition(Units);

            Units[0] = unit;
            ProcessUnits(composition);
            return Result.Create(composition, Messages);
        }

        protected override Unit CompileNodes(DyCodeModel codeModel, bool root)
        {
            if (!root)
                return base.CompileNodes(codeModel, root);
            else
                Messages.Clear();

            var oldCompiler = compiler;

            if (compiler == null)
                compiler = new DyCompiler(BuilderOptions, this);
            else
            {
                compiler = new DyCompiler(compiler);
                startOffset = composition.Units[0].Ops.Count;
            }

            var res = compiler.Compile(codeModel);

            if (res.Messages.Any())
                Messages.AddRange(res.Messages);

            if (!res.Success)
            {
                compiler = oldCompiler;
                startOffset = null;
                return null;
            }

            if (startOffset != null)
                res.Value.Layouts[0].Address = startOffset.Value;

            return res.Value;
        }
    }
}
