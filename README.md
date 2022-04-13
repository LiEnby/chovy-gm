# chovy-gm
At long last. GameMaker 8.1 to PSP!

What is this code?
Its Decompiled and Patched GameMaker Asset Compiler v1.0.98 (this was before there was any obfuscation)
it has been modified to exclusively produce PSP compatible files from GM81 Executables. 
Also a nice gui was added

NOTE: this release is really buggy, and there isnt much i can do about it,
this is mostly just using Karoshi (a PSP MINI put out by yoyogames back in 2011)  
as the "runner" for the game and adapting a early GMStudio Compiler to build files for it              
The "runner" was NEVER released publicly as an GM export module! and is basically was always in beta,                 
before being abandoned in favour of the PSVita version. dont expect stuff to work *well*           

the only bugs i can fix are ones relating to compilation, and not running the game itself.

(ISOs built with this WILL work in Chovy-Sign.)

# Dependancys
chovy-gm release zip. includes a few other executables
- at3tool it is an offical SONY tool for converting WAV into there properitary .at3 format. if anyone has a libary that can handle this please let me know.

- umdgenc is the offical UMD ISO builder from sony, it was the only UMD ISO builder i could find that would acturally work properly on the PSVita, (makes sense since its the offical tool) the reason most dont work is that they align sectors in whatever order. the PSP doesnt care, but PSPEmu on the PSVita does, im working on my own UMD builder to replace this.

- fluidsynth a MIDI synthesier, used for converting MID files inside the GM Executables into WAV and then into at3
src: https://github.com/FluidSynth/fluidsynth

- EBOOT.BIN same executable found in the PSP minis game "karoshi" it is effectively the gamemaker interpreter.
