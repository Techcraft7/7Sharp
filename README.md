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

```c
loop (10) {
  write(getLoopIndex() + 1);
}
```
#### What is `getLoopIndex()`

`getLoopIndex()` gets the current index inside the current `loop` loop **(DOES NOT WORK FOR `while` LOOP!)**

`getLoopIndex(n)` can get loop indexes outside of the current `loop` loop.

`getLoopIndex(n = 0)` is an abstraction for having to manage `i`, `j`, `k`, etc. because many beginners tend to mess this up, even though some interpreted languages (like Python) will let you have two for loops with `i` as the variable.

##### Example
```c
loop(5) {
    loop(5) {
        write("i = " + getLoopIndex(1) + " j = " + getLoopIndex());
    }
}
```
Outputs:
```
i = 0 j = 0
i = 0 j = 1
i = 0 j = 2
i = 0 j = 3
i = 0 j = 4
i = 0 j = 5
i = 1 j = 0
i = 1 j = 1
i = 1 j = 2
i = 1 j = 3
etc.
```

All of that out of the way, here is another example
### Fibonacci
```c
a = double(0);
b = double(1);
c = double(0);
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
# Beta Builds!

Check out our beta builds [here](https://github.com/Techcraft7/7SharpBetaBuilds)
