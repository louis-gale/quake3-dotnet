namespace Quake3.BotLib.IO
{
    public class Punctuation
    {
        public static Punctuation[] DefaultPunctuations =
        {
	        // binary operators
	        new Punctuation { Value = ">>=", SubType = PunctuationSubType.RightShiftAssign },
	        new Punctuation { Value = "<<=", SubType = PunctuationSubType.LeftShiftAssign },
	        new Punctuation { Value = "...", SubType = PunctuationSubType.Parameters },
	        // define merge operator
	        new Punctuation { Value = "##", SubType = PunctuationSubType.PrecompilerDirectiveMerge },
	        // logic operators
	        new Punctuation { Value = "&&", SubType = PunctuationSubType.LogicAnd },
	        new Punctuation { Value = "||", SubType = PunctuationSubType.LogicOr },
	        new Punctuation { Value = ">=", SubType = PunctuationSubType.LogicGeq },
	        new Punctuation { Value = "<=", SubType = PunctuationSubType.LogicLeq },
	        new Punctuation { Value = "==", SubType = PunctuationSubType.LogicEq },
	        new Punctuation { Value = "!=", SubType = PunctuationSubType.LogicUneq },
	        // arithmatic operators
	        new Punctuation { Value = "*=", SubType = PunctuationSubType.MultiplyAssign },
	        new Punctuation { Value = "/=", SubType = PunctuationSubType.DivideAssign },
	        new Punctuation { Value = "%=", SubType = PunctuationSubType.ModuloAssign },
	        new Punctuation { Value = "+=", SubType = PunctuationSubType.AddAssign },
	        new Punctuation { Value = "-=", SubType = PunctuationSubType.SubtractAssign },
	        new Punctuation { Value = "++", SubType = PunctuationSubType.Increment },
	        new Punctuation { Value = "--", SubType = PunctuationSubType.Decrement },
	        // binary operators
	        new Punctuation { Value = "&=", SubType = PunctuationSubType.BinaryAndAssign },
	        new Punctuation { Value = "|=", SubType = PunctuationSubType.BinaryOrAssign },
	        new Punctuation { Value = "^=", SubType = PunctuationSubType.BinaryXorAssign },
	        new Punctuation { Value = ">>", SubType = PunctuationSubType.RightShift },
	        new Punctuation { Value = "<<", SubType = PunctuationSubType.LeftShift },
	        // reference operators
	        new Punctuation { Value = "->", SubType = PunctuationSubType.PointerReference },
	        // C++
	        new Punctuation { Value = "::", SubType = PunctuationSubType.Cpp1 },
	        new Punctuation { Value = ".*", SubType = PunctuationSubType.Cpp2 },
	        // arithmatic operators
	        new Punctuation { Value = "*", SubType = PunctuationSubType.Multiply },
	        new Punctuation { Value = "/", SubType = PunctuationSubType.Divide },
	        new Punctuation { Value = "%", SubType = PunctuationSubType.Modulo },
	        new Punctuation { Value = "+", SubType = PunctuationSubType.Add },
	        new Punctuation { Value = "-", SubType = PunctuationSubType.Subtract },
	        new Punctuation { Value = "=", SubType = PunctuationSubType.Assign },
	        // binary operators
	        new Punctuation { Value = "&", SubType = PunctuationSubType.BinaryAnd },
	        new Punctuation { Value = "|", SubType = PunctuationSubType.BinaryOr },
	        new Punctuation { Value = "^", SubType = PunctuationSubType.BinaryXor },
	        new Punctuation { Value = "~", SubType = PunctuationSubType.BinaryNot },
	        // logic operators
	        new Punctuation { Value = "!", SubType = PunctuationSubType.LogicNot },
	        new Punctuation { Value = ">", SubType = PunctuationSubType.LogicGreater },
	        new Punctuation { Value = "<", SubType = PunctuationSubType.LogicLess },
	        // reference operator
	        new Punctuation { Value = ".", SubType = PunctuationSubType.Reference },
	        // seperators
	        new Punctuation { Value = ",", SubType = PunctuationSubType.Comma },
	        new Punctuation { Value = ";", SubType = PunctuationSubType.Semicolon },
	        // label indication
	        new Punctuation { Value = ":", SubType = PunctuationSubType.Colon },
	        // if statement
	        new Punctuation { Value = "?", SubType = PunctuationSubType.QuestionMark },
	        // embracements
	        new Punctuation { Value = "(", SubType = PunctuationSubType.ParenthesesOpen },
	        new Punctuation { Value = ")", SubType = PunctuationSubType.ParenthesesClose },
	        new Punctuation { Value = "{", SubType = PunctuationSubType.BraceOpen },
	        new Punctuation { Value = "}", SubType = PunctuationSubType.BraceClose },
	        new Punctuation { Value = "[", SubType = PunctuationSubType.SquareBracketOpen },
	        new Punctuation { Value = "]", SubType = PunctuationSubType.SquareBracketClose },
	        new Punctuation { Value = "\\", SubType = PunctuationSubType.BackSlash },
	        // precompiler operator
	        new Punctuation { Value = "#", SubType = PunctuationSubType.PrecompilerDirective },
	        new Punctuation { Value = "$", SubType = PunctuationSubType.Dollar }
        };

        /// <summary>
        /// Punctuation character(s)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Punctuation indication
        /// </summary>
        public PunctuationSubType SubType { get; set; }
    }
}