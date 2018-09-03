using System.Reflection;
using System.Text;
using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.Dynamic;
using log4net;

namespace MoonPad
{
    /// <summary>
    /// Provides an implementation of a <c>Lua</c> syntax language that can perform automatic outlining.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    internal class LuaDynamicSyntaxLanguage : DynamicOutliningSyntaxLanguage
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This constructor is for designer use only and should never be called by your code.
        /// </summary>
        public LuaDynamicSyntaxLanguage() { }

        /// <summary>
        /// Initializes a new instance of the <c>LuaDynamicSyntaxLanguage</c> class.
        /// </summary>
        /// <param name="key">The key of the language.</param>
        /// <param name="secure">Whether the language is secure.</param>
        public LuaDynamicSyntaxLanguage(string key, bool secure) : base(key, secure) { }

        /// <summary>
        /// Returns token parsing information for automatic outlining that determines if the current <see cref="IToken"/>
        /// in the <see cref="TokenStream"/> starts or ends an outlining node.
        /// </summary>
        /// <param name="tokenStream">A <see cref="TokenStream"/> that is positioned at the <see cref="IToken"/> requiring outlining data.</param>
        /// <param name="outliningKey">Returns the outlining node key to assign.  A <see langword="null"/> should be returned if the token doesn't start or end a node.</param>
        /// <param name="tokenAction">Returns the <see cref="OutliningNodeAction"/> to take for the token.</param>
        public override void GetTokenOutliningAction(TokenStream tokenStream, ref string outliningKey, ref OutliningNodeAction tokenAction)
        {
            // Get the token
            IToken token = tokenStream.Peek();
            //Log.Debug("token.Key = " + token.Key);

            // See if the token starts or ends an outlining node
            switch (token.Key)
            {
                case "ReservedWordToken":
                    Log.Debug("ReservedWordToken token.AutoCaseCorrectText = " + token.AutoCaseCorrectText);
                    switch (token.AutoCaseCorrectText)
                    {
                        case "if":
                        case "function":
                        case "for":
                        case "while":
                            // NOTE: for and while loops use 'do' keyword.
                            tokenAction = OutliningNodeAction.Start;
                            break;
                        case "end":
                            tokenAction = OutliningNodeAction.End;
                            break;
                    }
                    break;

                case "MultiLineCommentStartToken":
                    tokenAction = OutliningNodeAction.Start;
                    break;

                case "MultiLineCommentEndToken":
                    tokenAction = OutliningNodeAction.End;
                    break;

                case "LongBracketStringStartToken":
                    tokenAction = OutliningNodeAction.Start;
                    break;

                case "LongBracketStringEndToken":
                    tokenAction = OutliningNodeAction.End;
                    break;
            }
        }

        /// <summary>
        /// Allows for setting the collapsed text for the specified <see cref="OutliningNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="OutliningNode"/> that is requesting collapsed text.</param>
        public override void SetOutliningNodeCollapsedText(OutliningNode node)
        {
            TokenCollection tokens = node.Document.Tokens;
            int tokenIndex = tokens.IndexOf(node.StartOffset);

            switch (tokens[tokenIndex].Key)
            {
                case "ReservedWordToken":
                    {
                        if (node.Text.Substring(0, node.Text.IndexOf(' ')) == "function")
                        {
                            // Show the function name after the reserved word "function".
                            node.CollapsedText = node.Text.Substring(0, node.Text.IndexOf('('));
                        }
                        else
                        {
                            // Show the reserved word followed by " ..."
                            node.CollapsedText = node.Text.Substring(0, node.Text.IndexOf(' ')) + " ...";
                        }
                    }
                    break;

                case "MultiLineCommentStartToken":
                    {
                        // Add ... if the label string is truncated comment.
                        bool truncated = false;

                        // Remove the comment tags from the node text.
                        string comment = node.Text.Substring(4, node.Text.Length - 6).Trim();

                        // First line only.
                        if (comment.IndexOf('\n') > 0)
                        {
                            truncated = true;
                            comment = comment.Substring(0, comment.IndexOf('\n')).Trim();
                        }

                        // Restrict number of chars allowed in first line.
                        if (comment.Length > 36)
                        {
                            truncated = true;
                            comment = comment.Substring(0, 36);
                        }

                        // Set the label for the collapsed text.
                        StringBuilder sb = new StringBuilder();
                        sb.Append("--[[");
                        sb.Append(comment);
                        if (truncated) sb.Append("...");
                        sb.Append("]]");
                        node.CollapsedText = sb.ToString();
                    }
                    break;

                case "LongBracketStringStartToken":
                    {
                        node.CollapsedText = "[[...]]";
                    }
                    break;
            }
        }
    }
}
