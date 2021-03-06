﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Data
{
    class MefHelper
    {
        public static T Compose<T>()
        {
            using (var container = CreateContainer())
            {
                var exports = container.GetExports<T>().ToList();
                if (exports.Count == 0) throw new SimpleDataException("No ADO Provider found.");
                if (exports.Count > 1) throw new SimpleDataException("Multiple ADO Providers found; specify provider name or remove unwanted assemblies.");
                return exports.Single().Value;
            }
        }

        public static T Compose<T>(string contractName)
        {
            using (var container = CreateContainer())
            {
                var export = container.GetExport<T>(contractName);
                if (export == null) throw new ArgumentException("Unrecognised file.");
                return export.Value;
            }
        }

        private static CompositionContainer CreateContainer()
        {
            var path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            path = Path.GetDirectoryName(path);
            if (path == null) throw new ArgumentException("Unrecognised file.");

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var folderCatalog = new DirectoryCatalog(path, "Simple.Data.*.dll");
            return new CompositionContainer(new AggregateCatalog(assemblyCatalog, folderCatalog));
        }
    }
}
