#region Header Licence
//  ---------------------------------------------------------------------
//
//  Copyright (c) 2009 Alexandre Mutel and Microsoft Corporation.
//  All rights reserved.
//
//  This code module is part of AsmHighlighter, a plugin for visual studio
//  to provide syntax highlighting for x86 ASM language (.asm, .inc)
//
//  ------------------------------------------------------------------
//
//  This code is licensed under the Microsoft Public License.
//  See the file License.txt for the license details.
//  More info on: http://asmhighlighter.codeplex.com
//
//  ------------------------------------------------------------------
#endregion

// not used currently and references RegistrationAttribute which moved in VS2017
#if false

using System;
using Microsoft.VisualStudio.Shell;

namespace AsmHighlighter
{
    /// <summary>
    /// Code taken from http://phalanger.codeplex.com
    /// Unfortunately, the expression evaluator is not working. Don't know why...
    ///
    ///     This attribute registers Expression Evaluator for a package.  The attributes on a
    ///     package do not control the behavior of the package, but they can be used by registration
    ///     tools to register the proper information with Visual Studio.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    // Disable the "IdentifiersShouldNotHaveIncorrectSuffix" warning.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711")]
    public sealed class RegisterExpressionEvaluatorAttribute : RegistrationAttribute
    {
        /// <summary>CTor</summary>
        /// <param name="type">Type to be registered as expression evaluator - must have <see cref="GuidAttribute"/> and should implement <see cref="IDebugExpressionEvaluator2"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <param name="LanguageGuid">String representing GUID of language to register evaluator for</param>
        /// <param name="VendorGuid">String represenring GUID of vendor of language to register evakluator for</param>
        public RegisterExpressionEvaluatorAttribute(Type type, string LanguageGuid, string VendorGuid)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            this.type = type;
            this.languageGuid = new Guid(LanguageGuid);
            this.vendorGuid = new Guid(VendorGuid);
        }
        /// <summary>Contains value of the <see cref="VendorGuid"/> property</summary>
        private readonly Guid vendorGuid;
        /// <summary>Gets GUID of vendor to register expressiob evaluator for</summary>
        public Guid VendorGuid { get { return vendorGuid; } }

        /// <summary>Contains value of the <see cref="LanguageGuid"/> property</summary>
        private readonly Guid languageGuid;
        /// <summary>Gets guid of language to register expression evaluator for</summary>
        public Guid LanguageGuid { get { return languageGuid; } }
        /// <summary>Contains value of the <see cref="Type"/> property</summary>
        private readonly Type type;
        /// <summary>Gets type registered by this attribute</summary>
        public Type Type { get { return type; } }
        private static readonly Guid guidCOMPlusOnlyEng = new Guid("449EC4CC-30D2-4032-9256-EE18EB41B62B");
        private static readonly Guid guidCOMPlusNativeEng = new Guid("92EF0900-2251-11D2-B72E-0000F87572EF");
        private static readonly Guid guidNativeOnlyEng = new Guid("3B476D35-A401-11D2-AAD4-00C04F990171");
        /// <summary>Provides registration information about a VSPackage when called by an external registration tool such as regpkg.exe.</summary>
        /// <param name="context">A registration context provided by an external registration tool. The context can be used to create registry keys, log registration activity, and obtain information about the component being registered. </param>
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
#if DEBUG
            Console.WriteLine("Registering AsmHighlighter Expression Evaluator");
#endif
            if (context == null) return;
            using (Key rk = context.CreateKey(string.Format("AD7Metrics\\ExpressionEvaluator\\{0:B}\\{1:B}", LanguageGuid, vendorGuid)))
            {
                rk.SetValue("CLSID", Type.GUID.ToString("B"));
                rk.SetValue("Language", "ASM Language");
                rk.SetValue("Name", "AsmHighlighter");
                using (Key rk2 = rk.CreateSubkey("Engine"))
                {
                    rk2.SetValue("0", guidCOMPlusOnlyEng.ToString("B"));
                    rk2.SetValue("1", guidCOMPlusNativeEng.ToString("B"));
                    rk2.SetValue("2", guidNativeOnlyEng.ToString("B"));
                }
            }
        }
        /// <summary>Removes registration information about a VSPackage when called by an external registration tool such as regpkg.exe.</summary>
        /// <param name="context">A registration context provided by an external registration tool. The context can be used to remove registry keys, log registration activity, and obtain information about the component being registered. </param>
        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            if (context == null) return;
#if DEBUG
            Console.WriteLine("Unregistering AsmHighlighter Expression Evaluator");
#endif
            context.RemoveKey(string.Format("AD7Metrics\\ExpressionEvaluator\\{0:B}\\{1:B}", languageGuid, vendorGuid));
            context.RemoveKeyIfEmpty(string.Format("AD7Metrics\\ExpressionEvaluator\\{0:B}", languageGuid));
        }
    }

}
#endif