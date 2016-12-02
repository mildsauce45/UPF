# UPF - Unity Presentation Framework
A Unity GUI framework based on WPF.

## Description

This package attempts to create a unified system for putting together GUI components in Unity. All my previous work uses ad hoc written
components that shared much, but not enough code to display. I found myself wanting a standard UI framework and I'm pretty familiar with
WPF, so I decided to give it a shot creating something along those lines. The is a first pass attempt and is lacking many of the niceties
of the WPF framework, most notably templating, but it solves my basic needs for the moment.

### Examples

![img](https://github.com/mildsauce45/UPF/blob/master/Examples/Images/CombatScreen.png)

### Next Up

* Templating - For my own use, this isn't too necessary, but in order to turn this into a general purpose library I'd say it's required
* ICommand - When CanExecute returns false, controls need to disable automatically.

### Future Improvements

* Use the new GUI system - All of my work has been with the legacy unity gui system, so I just used that. It would be nice to be able to choose between the two with a switch
