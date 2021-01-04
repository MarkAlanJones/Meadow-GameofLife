# Meadow-GameofLife
<a href="https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life">Conway's Game of Life</a>
is a cellular automaton devised by the late mathematician John Conway.

Use the SPI LCD display to show the game of life with a Meadow F7 dev board.

use standard wiring for Meadow F7 and LCD
![Meadow Frizing](/MeadowGameofLife/st7789_fritzing.jpg)

Implementation notes:
* the display is 240x240 but I use a larger 8x8 pixel so the life board is 30x30
* the colour is changed each generation, and the onboard LED is set to "match"
* by saving display.show until the end of the loop you can get almost 2 frames a second, displaying after each pixel is very slow
(with beta 4.3 3-5 frames/s)
