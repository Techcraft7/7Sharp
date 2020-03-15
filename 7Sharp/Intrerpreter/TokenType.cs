using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sly.lexer;
using sly.parser.generator;

namespace _7Sharp.Intrerpreter
{
	public enum TokenType
	{
		[Lexeme(GenericToken.Double)]
		DOUBLE,
		[Lexeme(GenericToken.Int)]
		INT,
		[Lexeme(GenericToken.String, "\"", "\\")]
		STRING,
		[Lexeme(GenericToken.Identifier, IdentifierType.AlphaNumericDash)]
		IDENTIFIER,
		[Lexeme(GenericToken.SugarToken, ";")]
		SEMICOLON,
		[Lexeme(GenericToken.SugarToken, ",")]
		COMMA,
		[Lexeme(GenericToken.SugarToken, "=")]
		ASSIGNMENT,
		[Lexeme(GenericToken.SugarToken, "(")]
		LPAREN,
		[Lexeme(GenericToken.SugarToken, ")")]
		RPAREN,
		[Lexeme(GenericToken.SugarToken, "{")]
		LBRACE,
		[Lexeme(GenericToken.SugarToken, "}")]
		RBRACE,
		[Lexeme(GenericToken.SugarToken, ">")]
		GREATER,
		[Lexeme(GenericToken.SugarToken, "<")]
		LESSER,
		[Lexeme(GenericToken.SugarToken, "==")]
		DOUBLEEQUALS,
		[Lexeme(GenericToken.SugarToken, "!=")]
		DIFFERENT,
		[Lexeme(GenericToken.SugarToken, "+")]
		PLUS,
		[Lexeme(GenericToken.SugarToken, "-")]
		MINUS,
		[Lexeme(GenericToken.SugarToken, "*")]
		TIMES,
		[Lexeme(GenericToken.SugarToken, "/")]
		DIVIDE,
		[Lexeme(GenericToken.SugarToken, "++")]
		PLUSPLUS,
		[Lexeme(GenericToken.SugarToken, "--")]
		MINUSMINUS,
		[Lexeme(GenericToken.KeyWord, "if")]
		IF,
		[Lexeme(GenericToken.KeyWord, "else")]
		ELSE,
		[Lexeme(GenericToken.KeyWord, "loop")]
		LOOP,
		[Lexeme(GenericToken.KeyWord, "while")]
		WHILE,
		[Lexeme(GenericToken.KeyWord, "true")]
		TRUE,
		[Lexeme(GenericToken.KeyWord, "false")]
		FALSE,
		[Lexeme(GenericToken.KeyWord, "break")]
		BREAK,
		[Lexeme(GenericToken.KeyWord, "continue")]
		CONTINUE,
		[Lexeme(GenericToken.KeyWord, "return")]
		RETURN,
		[Lexeme(GenericToken.KeyWord, "function")]
		FUNCTION,
		[Comment("//", "/*", "*/")]
		COMMENT
	}
}
