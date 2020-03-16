![Logo](logo.png)
# 7Sharp

The free, open source, and simple programming language

## Features

- Built in shell to load, save, edit, and run 7Sharp code
- C-style code
- Simple to learn
- No scoping (might add later)
- Setting colors `fgColor()` and `bgColor`
  - See `System.ConsoleColor` for color names!

## Sample code

### Count to 10

```
loop (10) {
  write(getLoopIndex() + 1);
}
```
#### What is `getLoopIndex()`?
`getLoopIndex()` is a function to get the index in the loop statement running. It automatically accounts for nested loops, so you don't need to manage any i's, j's, k's, etc. If you look at `Interpreter.cs` you will find this:
```
int times = (int)Evaluate(args); // in the "Count to 10" example it is just 10, but it can be an expression!
loopIndexes.Push(0); // add a new loop index
for (int j = 0; j < times && !exitLoop; j++) //exit loop is for "break"
{
  InternalRun(inside, out _, false); //run the code without resetting the evaluator
  loopIndexes.Push(loopIndexes.Pop() + 1); //increment the current loop index by 1
  skipLoop = false; // used for "continue"
}
_ = loopIndexes.Pop(); //remove loop index because we are done with it
```
All of that out of the way, here is another example
### Fibonacci
```
a = 0.0; //double so we can go BIG
b = 1.0;
c = 0.0;
loop (50) {
  c = a + b;
  b = a;
  a = c;
  write(c);
}
```
#### Output:
```
1
1
2
3
5
...
```