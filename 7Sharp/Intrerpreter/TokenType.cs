﻿using System;
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
		[Lexeme(GenericToken.Identifier, IdentifierType.AlphaNumericDash)]
		IDENTIFIER,
		[Lexeme(GenericToken.SugarToken, "=")]
		ASSIGNMENT,
		[Lexeme(GenericToken.SugarToken, "(")]
		LPAREN,
		[Lexeme(GenericToken.SugarToken, ")")]
		RPAREN,
		[Lexeme(GenericToken.SugarToken, ">")]
		GREATER,
		[Lexeme(GenericToken.SugarToken, "<")]
		LESSER,
		[Lexeme(GenericToken.SugarToken, "==")]
		EQUALS,
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
		[Lexeme(GenericToken.KeyWord, "if")]
		IF,
		[Lexeme(GenericToken.KeyWord, "loop")]
		LOOP,
		[Lexeme(GenericToken.KeyWord, "while")]
		WHILE,
		[Lexeme(GenericToken.KeyWord, "true")]
		TRUE,
		[Lexeme(GenericToken.KeyWord, "false")]
		FALSE,
	}
}