﻿using Dyalect.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dyalect
{
    public sealed class DyaOptions
    {
        private const string COMPILER = "Compiler settings";
        private const string LINKER = "Linker settings";
        private const string GENERAL = "General settings";

        [Binding(Help = "A full path to the .dy file which should be executed (or to the file or directory with files in '-test' mode).", Category = COMPILER)]
        public string FileName { get; set; }

        [Binding("debug", Help = "Compile in debug mode.", Category = COMPILER)]
        public bool Debug { get; set; }

        [Binding("nowarn", Help = "Do not generate warnings.", Category = COMPILER)]
        public bool NoWarnings { get; set; }

        [Binding("nolang", Help = "Do not import \"lang\" module that includes basic primitives and operations.", Category = COMPILER)]
        public bool NoLang { get; set; }

        [Binding("path", Help = "A path where linker would look for referenced modules. You can specify this switch multiple times.", Category = LINKER)]
        public string[] Paths { get; set; }

        [Binding("nologo", Help = "Do not show logo.", Category = GENERAL)]
        public bool NoLogo { get; set; }

        [Binding("time", Help = "Measure execution time.", Category = GENERAL)]
        public bool MeasureTime { get; set; }

        [Binding("test", Help = "Run unit tests from a file (or files if a path to a directory is specified). Usage: dya [path to file or directory] -test.", Category = GENERAL)]
        public bool Test { get; set; }

        [Binding("appveyor")]
        public bool AppVeyour { get; set; }

        [Binding("i", Help = "Stay in interactive mode after executing a file.", Category = GENERAL)]
        public bool StayInInteractive { get; set; }

        public override string ToString()
        {
            var list = new List<(string, string)>();
            
            foreach (var pi in typeof(DyaOptions).GetProperties())
            {
                var attr = Attribute.GetCustomAttribute(pi, typeof(BindingAttribute)) as BindingAttribute;

                if (attr != null)
                {
                    var val = pi.GetValue(this);
                    var byt = pi.PropertyType == typeof(bool);
                    var i4 = pi.PropertyType == typeof(int);

                    if ((byt && !(bool)val)
                        || (i4 && (int)val == 0)
                        || ReferenceEquals(val, null))
                        continue;

                    var key = attr.Names?.Length > 0 ? attr.Names[0] : "<file name>";
                    list.Add((key, byt ? "" 
                        : val is System.Collections.IEnumerable 
                            ? string.Join(';', ((System.Collections.IEnumerable)val).OfType<object>().Select(v => v.ToString()))
                        : val.ToString()));
                }
            }

            var max = list.Max(e => e.Item1.Length) + 1;
            var sb = new StringBuilder();

            foreach (var (k,v) in list)
            {
                if (sb.Length > 0)
                    sb.AppendLine();

                sb.Append(("-" + k ).PadRight(max, ' ') + " ");
                sb.Append(v);
            }

            return sb.ToString();
        }
    }
}
