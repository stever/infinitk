﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MoonPad.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MoonPad.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;SyntaxLanguage Key=&quot;Lua&quot; LanguageDefinitionVersion=&quot;4.0&quot; Secure=&quot;True&quot;
        ///				SyntaxLanguageTypeName=&quot;MoonPad.LuaDynamicSyntaxLanguage, MoonPad&quot;
        ///				xmlns=&quot;http://ActiproSoftware/SyntaxEditor/4.0/LanguageDefinition&quot;&gt;
        ///
        ///	&lt;!-- String Properties --&gt;
        ///	&lt;Properties&gt;
        ///		&lt;Property Key=&quot;Creator&quot; Value=&quot;Actipro Software LLC&quot; /&gt;
        ///		&lt;Property Key=&quot;Copyright&quot; Value=&quot;Copyright (c) 2001-2009 Actipro Software LLC.  All rights reserved.&quot; /&gt;
        ///	&lt;/Properties&gt;
        ///
        ///	&lt;!-- Highlighting Styles --&gt;
        ///	&lt;Styles&gt;
        ///		&lt;Style Key=&quot;Reserve [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ActiproSoftware_Lua {
            get {
                return ResourceManager.GetString("ActiproSoftware_Lua", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] htdocs_fonts_PragmataProMono_woff2 {
            get {
                object obj = ResourceManager.GetObject("htdocs_fonts_PragmataProMono_woff2", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*! jQuery v3.2.1 | (c) JS Foundation and other contributors | jquery.org/license */
        ///!function(a,b){&quot;use strict&quot;;&quot;object&quot;==typeof module&amp;&amp;&quot;object&quot;==typeof module.exports?module.exports=a.document?b(a,!0):function(a){if(!a.document)throw new Error(&quot;jQuery requires a window with a document&quot;);return b(a)}:b(a)}(&quot;undefined&quot;!=typeof window?window:this,function(a,b){&quot;use strict&quot;;var c=[],d=a.document,e=Object.getPrototypeOf,f=c.slice,g=c.concat,h=c.push,i=c.indexOf,j={},k=j.toString,l=j.hasOwnProperty,m=l.toStrin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htdocs_js_jquery_3_2_1_min_js {
            get {
                return ResourceManager.GetString("htdocs_js_jquery_3_2_1_min_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to body {
        ///    padding: 0;
        ///    margin: 0;
        ///}
        ///
        ///@font-face {
        ///    font-family: &apos;PragmataPro Mono&apos;;
        ///    src: url(&apos;../fonts/PragmataProMono.woff2&apos;) format(&apos;woff2&apos;);
        ///}
        ///
        ///.terminal {
        ///    padding: 2px;
        ///    --size: 1.2;
        ///}
        ///
        ///.terminal, .cmd, .terminal .terminal-output div div, .cmd .prompt {
        ///    font-family: &apos;PragmataPro Mono&apos;, monospace;
        ///}
        ///.
        /// </summary>
        internal static string htdocs_term_index_css {
            get {
                return ResourceManager.GetString("htdocs_term_index_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;en&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;script src=&quot;../js/jquery-3.2.1.min.js&quot;&gt;&lt;/script&gt;
        ///    &lt;script src=&quot;jquery.terminal.min.js&quot;&gt;&lt;/script&gt;
        ///    &lt;link rel=&quot;stylesheet&quot; href=&quot;jquery.terminal.min.css&quot; /&gt;
        ///    &lt;link rel=&quot;stylesheet&quot; href=&quot;index.css&quot; /&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;term_demo&quot;&gt;&lt;/div&gt;
        ///    &lt;script src=&quot;index.js&quot;&gt;&lt;/script&gt;
        ///&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string htdocs_term_index_html {
            get {
                return ResourceManager.GetString("htdocs_term_index_html", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to jQuery(function ($, undefined) {
        ///    $(&apos;#term_demo&apos;).terminal(&apos;/api/method/json-rpc&apos;, {
        ///        height: &apos;100vh&apos;,
        ///        width: &apos;100vw&apos;,
        ///        prompt: &apos;&gt; &apos;,
        ///        greetings: &apos;&apos;
        ///    });
        ///});
        ///.
        /// </summary>
        internal static string htdocs_term_index_js {
            get {
                return ResourceManager.GetString("htdocs_term_index_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*!
        /// *       __ _____                     ________                              __
        /// *      / // _  /__ __ _____ ___ __ _/__  ___/__ ___ ______ __ __  __ ___  / /
        /// *  __ / // // // // // _  // _// // / / // _  // _//     // //  \/ // _ \/ /
        /// * /  / // // // // // ___// / / // / / // ___// / / / / // // /\  // // / /__
        /// * \___//____ \\___//____//_/ _\_  / /_//____//_/ /_/ /_//_//_/ /_/ \__\_\___/
        /// *           \/              /____/                              version 1.21.0
        /// * http://terminal.jcubic.pl
        /// *
        /// * [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htdocs_term_jquery_terminal_min_css {
            get {
                return ResourceManager.GetString("htdocs_term_jquery_terminal_min_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /**@license
        /// *       __ _____                     ________                              __
        /// *      / // _  /__ __ _____ ___ __ _/__  ___/__ ___ ______ __ __  __ ___  / /
        /// *  __ / // // // // // _  // _// // / / // _  // _//     // //  \/ // _ \/ /
        /// * /  / // // // // // ___// / / // / / // ___// / / / / // // /\  // // / /__
        /// * \___//____ \\___//____//_/ _\_  / /_//____//_/ /_/ /_//_//_/ /_/ \__\_\___/
        /// *           \/              /____/                              version 1.21.0
        /// *
        /// * This file is part of [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htdocs_term_jquery_terminal_min_js {
            get {
                return ResourceManager.GetString("htdocs_term_jquery_terminal_min_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] PragmataPro_Mono_R_0826 {
            get {
                object obj = ResourceManager.GetObject("PragmataPro_Mono_R_0826", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] PragmataPro_Mono_R_liga_0826 {
            get {
                object obj = ResourceManager.GetObject("PragmataPro_Mono_R_liga_0826", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] ProFontWindows {
            get {
                object obj = ResourceManager.GetObject("ProFontWindows", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}