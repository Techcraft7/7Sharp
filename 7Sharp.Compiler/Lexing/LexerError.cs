﻿namespace _7Sharp.Compiler.Lexing;

public readonly record struct LexerError(LexerErrorType Type, FileLocation Location, int Length, string? Data = null);

public enum LexerErrorType
{
	INVALID_CHAR,
	MISSING_SEMICOLON,
	MISSING_OPEN_PAREN,
	MISSING_CLOSE_PAREN,
	MISSING_OPEN_BRACE,
	MISSING_CLOSE_BRACE,
	MISSING_OPEN_BRACKET,
	MISSING_CLOSE_BRACKET,
	UNCLOSED_MULTILINE_COMMENT,
	INTEGER_WITH_DECIMAL_POINT,
	INVALID_NUMBER_SIZE,
	MULTIPLE_DECIMAL_POINTS,
	NEGATIVE_UINT,
	INVALID_CHAR_LITERAL,
	INVALID_ESCAPE_SEQUENCE,
	UNCLOSED_CHAR,
	UNCLOSED_STRING,
}