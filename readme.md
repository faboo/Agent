Agent
=====

Agent is a scratchpad and small command agent. Editing text works with
vi-like commands. Agent is intended to run the background so it can be brought
up quickly with a keyboard shortcut (see below). The text of the scratchpad is
saved to disk whenever Agent loses focus however, should you need to shut it
down or reboot. It also supports syntax highilighting, and a simple command language.

The Keyboard
------------

Like vi, Agent is very keyboard-centric. 

Commands
--------

To run a command from the scratchpad, simply enter its name followed by a colon
and hit the &lt;Enter&gt; key from normal mode. If the command takes an argument, it
is supplied after the colon. If the command takes additional arguments, they are
supplied on the following lines by typing some space, the name of the argument,
a colon, and then the intended value for the argument. Don't worry, hitting
enter while in insert mode will fill in any arguments the current command.

Take the remember command for example:

	remember: work items
	  this: Call Dana about the uptime issue

An important command to remember is help. Run help with no arguments to get a
list of all the commands available to you.
