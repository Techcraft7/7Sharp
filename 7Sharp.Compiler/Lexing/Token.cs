namespace _7Sharp.Compiler.Lexing;

public readonly record struct Token(TokenType Type, string Value, FileLocation Location);
