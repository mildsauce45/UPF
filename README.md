# UPF - Unity Presentation Framework
A Unity GUI framework based on WPF.

## Description

This package attempts to create a unified system for putting together GUI components in Unity. All my previous work uses ad hoc written
components that shared much, but not enough code to display. I found myself wanting a standard UI framework and I'm pretty familiar with
WPF, so I decided to give it a shot creating something along those lines. The is a first pass attempt and is lacking many of the niceties
of the WPF framework, most notably templating and data binding, but it solves my basic needs for the moment.

### Needed Improvements

These things I'll actually need for my current projects.

* Invalidate Layout - Recalculate/redraw when properties change (simple one way databinding)

### Future Improvements

* Full fledged data binding - Not necessary right now as I have no need for even simple text input boxes, but this would be the first big improvement I'd make
* XAML-like markup - Much of WPF's ease of use comes from this and it would be nice to have
* Templating - For my own use, this isn't too necessary, but in order to turn this into a general purpose library I'd say it's required
* Use the new GUI system - All of my work has been with the legacy unity gui system, so I just used that. It would be nice to be able to choose between the two with a switch