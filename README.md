![Logo](logo.png)
# 7Sharp

The free, open source, and simple programming language

## Features

- Built in shell to load, save, edit, and run 7Sharp code
- C-style code
- Simple to learn
- Setting colors `fgColor()` and `bgColor()`

## Sample code

### Count to 10

```
loop (10) {
  write(getLoopIndex() + 1);
}
```
#### What is `getLoopIndex()`

`getLoopIndex()` gets the current index inside the current `loop` loop **(DOES NOT WORK FOR `while` LOOP!)**

`getLoopIndex(n)` can get loop indexes outside of the current `loop` loop.

`getLoopIndex(n = 0)` is an abstraction for having to manage `i`, `j`, `k`, etc. because many beginners tend to mess this up, even though some interpreted languages (like Python) will let you have two for loops with `i` as the variable.

All of that out of the way, here is another example
### Fibonacci
```
a = 0.0; // double so we can go BIG
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
