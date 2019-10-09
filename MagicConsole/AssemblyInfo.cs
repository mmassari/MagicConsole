using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

namespace MagicConsole
{
    public class AssemblyInfo : Dictionary<InfoItem,string>
    {
        // Constructors.
        public AssemblyInfo()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public AssemblyInfo(Assembly assembly)
        {
            Add(InfoItem.fullpath, Path.GetDirectoryName(assembly.Location));
            Add(InfoItem.filename, Path.GetFileName(assembly.Location));
            // Get values from the assembly.
            AssemblyTitleAttribute titleAttr =
                GetAssemblyAttribute<AssemblyTitleAttribute>(assembly);
            if (titleAttr != null)
                Add(InfoItem.title, titleAttr.Title);
            else
                Add(InfoItem.title, string.Empty);

            AssemblyDescriptionAttribute assemblyAttr =
                GetAssemblyAttribute<AssemblyDescriptionAttribute>(assembly);
            if (assemblyAttr != null)
                Add(InfoItem.description, assemblyAttr.Description);
            else
                Add(InfoItem.description, string.Empty);

            AssemblyCompanyAttribute companyAttr =
                GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly);
            if (companyAttr != null)
                Add(InfoItem.company, companyAttr.Company);
            else
                Add(InfoItem.company, string.Empty);


            AssemblyProductAttribute productAttr =
                GetAssemblyAttribute<AssemblyProductAttribute>(assembly);
            if (productAttr != null)
                Add(InfoItem.product, productAttr.Product);
            else
                Add(InfoItem.product, string.Empty);

            AssemblyCopyrightAttribute copyrightAttr =
                GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly);
            if (copyrightAttr != null)
                Add(InfoItem.copyright, copyrightAttr.Copyright);
            else
                Add(InfoItem.copyright, string.Empty);

            AssemblyTrademarkAttribute trademarkAttr =
                GetAssemblyAttribute<AssemblyTrademarkAttribute>(assembly);
            if (trademarkAttr != null)
                Add(InfoItem.trademark, trademarkAttr.Trademark);
            else
                Add(InfoItem.trademark, string.Empty);

            Add(InfoItem.assemblyVersion, assembly.GetName().Version.ToString());

            AssemblyFileVersionAttribute fileVersionAttr =
                GetAssemblyAttribute<AssemblyFileVersionAttribute>(assembly);
            if (fileVersionAttr != null)
                Add(InfoItem.fileVersion, fileVersionAttr.Version);
            else
                Add(InfoItem.fileVersion, string.Empty);

            GuidAttribute guidAttr = GetAssemblyAttribute<GuidAttribute>(assembly);
            if (guidAttr != null)
                Add(InfoItem.guid, guidAttr.Value);
            else
                Add(InfoItem.guid, string.Empty);

            NeutralResourcesLanguageAttribute languageAttr =
                GetAssemblyAttribute<NeutralResourcesLanguageAttribute>(assembly);
            if (languageAttr != null)
                Add(InfoItem.language, languageAttr.CultureName);
            else
                Add(InfoItem.language, string.Empty);
        }

        // Return a particular assembly attribute value.
        public static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            // Get attributes of this type.
            object[] attributes = assembly.GetCustomAttributes(typeof(T), true);

            // If we didn't get anything, return null.
            if ((attributes == null) || (attributes.Length == 0)) return null;

            // Convert the first attribute value into the desired type and return it.
            return (T)attributes[0];
        }
    }
}
