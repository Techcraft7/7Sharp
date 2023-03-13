namespace _7Sharp.Compiler.Lexing;

public enum TokenType
{
	// Comments
	// "// ..." or "/* ... */"
	SINGLE_LINE_COMMENT,
	MULTI_LINE_COMMENT,
	
	SEMICOLON,
	IDENFITIER,

	// Delimeters
	// ()
	OPEN_PAREN,
	CLOSE_PAREN,
	// {}
	OPEN_BRACE,
	CLOSE_BRACE,
	// []
	OPEN_BRACKET,
	CLOSE_BRACKET,
	
	// Literals
	TRUE,
	FALSE,
	INTEGER,
	FLOAT,
	CHAR,
	STRING,
	EMPTY,

	// Keywords
	FUNCTION,
	IF,
	ELSE,
	WHILE,

	// Operators
	// Math
	PLUS,
	MINUS,
	TIMES,
	DIVIDE,
	MOD,
	INCREMENT,
	DECREMENT,
	// Bool
	EQUALS,
	NOT_EQUALS,
	BOOL_AND,
	BOOL_OR,
	BOOL_XOR,
	BOOL_NOT,
	GREATER_THAN,
	LESS_THAN,
	GREATER_THAN_OR_EQUAL,
	LESS_THAN_OR_EQUAL,
	// Bit
	BIT_AND,
	BIT_OR,
	BIT_XOR,
	BIT_NOT,
	BIT_LEFT,
	BIT_RIGHT,
	// Other
	ASSIGNMENT, // =
	DOT, // .
	DEFAULT, // ??
	ERROR_MAP, // !>>
	ERROR_DEFAULT, // !??
	VALUE_CHAIN, // ?.
	ERROR_CHAIN, // !.
	CATCH_ALL, // _
	LAMBDA, // =>
}
