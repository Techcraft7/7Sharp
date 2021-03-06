﻿using _7Sharp.Interpreter.Nodes.Attributes;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Sharp.Interpreter.Nodes
{
	internal enum ExpressionType
	{
		FUNCTION_CALL,			// write("hi");
		ASSIGNMENT,				// x = 5;
		INCREMENT,				// x++;
		DECREMENT,				// x--;
		[Block]
		IF,						// if (condition) { OR else if (condition) {
		[Block]
		ELSE,					// else {
		[Block]
		LOOP,					// loop (times) {
		[Block]
		WHILE,					// while (condition) {
		[Block]
		[DontRun]
		FUNCTION_DEFINITION,	// function func(param1, param2, ...) {
		BREAK,					// break;
		CONTINUE,				// continue;
		RETURN,					// return; OR return value;
		[DontRun]
		IMPORT					// import "lib";
	}
	internal static class ExpressionTypeExtensions
	{
		private static List<ExpressionType> BLOCKS = new List<ExpressionType>();
		private static List<ExpressionType> DONT_RUN = new List<ExpressionType>();

		static ExpressionTypeExtensions()
		{
			BLOCKS.AddRange(typeof(ExpressionType).GetFields().Where(f => f.GetCustomAttributes(false).Any(a => a.GetType() == typeof(BlockAttribute))).Select(f => (ExpressionType)f.GetValue(null)));
			DONT_RUN.AddRange(typeof(ExpressionType).GetFields().Where(f => f.GetCustomAttributes(false).Any(a => a.GetType() == typeof(DontRunAttribute))).Select(f => (ExpressionType)f.GetValue(null)));
		}

		public static Node GetNode(this ExpressionType et, List<Token<TokenType>> expr, LexerPosition exprPos, ref InterpreterState state)
		{
			if (et.IsBlock())
			{
				bool hasArg = et != ExpressionType.ELSE;
				List<List<Token<TokenType>>> args = new List<List<Token<TokenType>>>();
				if (hasArg)
				{
					args = Interpreter.GetArgs(expr);
				}
				switch (et)
				{
					case ExpressionType.ELSE:
						return new ElseNode(exprPos);
					case ExpressionType.IF:
						args.ThrowIfNotSize(exprPos, 1);
						return new IfNode(args.First(), IfNode.IsElseIf(expr), exprPos);
					case ExpressionType.LOOP:
						args.ThrowIfNotSize(exprPos, 1);
						return new LoopNode(args.First(), exprPos);
					case ExpressionType.WHILE:
						args.ThrowIfNotSize(exprPos, 1);
						return new WhileNode(args.First(), exprPos);
					case ExpressionType.FUNCTION_DEFINITION:
						return new FunctionDefinitionNode(
							expr[1].StringWithoutQuotes,
							args.Select(a => a.AsString()).ToArray(),
							exprPos
						);
				}
			}
			else
			{
				switch (et)
				{
					case ExpressionType.IMPORT:
						return new ImportNode(expr[1].StringWithoutQuotes, exprPos);
					case ExpressionType.RETURN:
						return new ReturnNode(expr.Skip(1).Reverse().Skip(1).Reverse().ToList(), exprPos);
					case ExpressionType.BREAK:
						return new BreakNode(exprPos);
					case ExpressionType.CONTINUE:
						return new ContinueNode(exprPos);
					case ExpressionType.FUNCTION_CALL:
						string name = expr[0].StringWithoutQuotes;
						bool isSysFunc = state.Functions.Any(f => f.Name == name);
						if (isSysFunc)
						{
							return new FunctionCallNode(
								state.Functions.First(f => f.Name.Equals(name)),
								Interpreter.GetArgs(expr),
								exprPos
							);
						}
						else if (state.UserFuncs.Any(f => f.Name == name))
						{
							return new FunctionCallNode(
								state.UserFuncs.First(f => f.Name == name),
								Interpreter.GetArgs(expr),
								exprPos
							);
						}
						throw new InterpreterException($"Unkown function \"{name}\" at {exprPos}");
					case ExpressionType.ASSIGNMENT:
						bool isArray = AssignmentNode.IsArrayAssignment(expr, out bool isIndexer);
						return new AssignmentNode(
							expr[0].StringWithoutQuotes,
							isIndexer ?
							AssignmentNode.GetValueOfIndexerAssignment(expr) :
							expr.Skip(2).Reverse().Skip(1).Reverse().ToList(),
							isArray,
							isIndexer,
							isIndexer ? AssignmentNode.GetIndex(expr) : new List<Token<TokenType>>(),
							exprPos
						);
					case ExpressionType.INCREMENT:
						return new IncrementNode(expr[0].StringWithoutQuotes, exprPos);
					case ExpressionType.DECREMENT:
						return new DecrementNode(expr[0].StringWithoutQuotes, exprPos);
				}
			}
			return null;
		}

		public static bool IsBlock(this ExpressionType et) => BLOCKS.Contains(et);

		public static bool Matches(this ExpressionType et, List<Token<TokenType>> expr)
		{
			switch (et)
			{
				case ExpressionType.IMPORT:
					return ImportNode.IsImport(expr);
				case ExpressionType.FUNCTION_DEFINITION:
					return FunctionDefinitionNode.IsFunctionDefinition(expr);
				case ExpressionType.FUNCTION_CALL:
					return FunctionCallNode.IsFunctionCall(expr);
				case ExpressionType.ASSIGNMENT:
					return AssignmentNode.IsAssignment(expr, out _);
				case ExpressionType.INCREMENT:
					return IncrementNode.IsIncrement(expr);
				case ExpressionType.DECREMENT:
					return DecrementNode.IsDecrement(expr);
				case ExpressionType.IF:
					return IfNode.IsIf(expr) || IfNode.IsElseIf(expr);
				case ExpressionType.ELSE:
					return ElseNode.IsElse(expr);
				case ExpressionType.LOOP:
					return LoopNode.IsLoopNode(expr);
				case ExpressionType.WHILE:
					return WhileNode.IsWhile(expr);
				case ExpressionType.RETURN:
					return ReturnNode.IsReturn(expr);
				case ExpressionType.BREAK:
					return BreakNode.IsBreak(expr);
				case ExpressionType.CONTINUE:
					return ContinueNode.IsContinue(expr);
			}
			return false;
		}

		public static bool WillRun(this ExpressionType et) => !DONT_RUN.Contains(et);
	}
}
