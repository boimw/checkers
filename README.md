University of Novi Sad
Faculty of Technical Science

Soft Computing 2015/2016
( https://github.com/ftn-ai-lab ) 

Team members: 
Marina Nenic RA 109/2012, 
Jana Vojnovic RA 164/2012 and
Bojan Marjanovic RA 185/2012

# Checkers

Creating a bot for the game Checkers

  Checkers or Draughts is a strategy board game for two players, played on an 8×8 chess board. Each player has 12 figureens and moves one at a time on the board. Only move allowed is diagonal, and only forward. When a player jumps across the other player’s figure, that one is consider “eaten” and is moved from the board. Multiple jumps are allowed, but only one figurren at the time. When a player reaches the end of the board towards his opponent, he gets a king figureen. King can move backwards and forward. The winner is the one that eats or blocks the opponent so he has no moves.

There are 4 steps to creating a game bot:

1. Creating a game state representation. This includes the logic for identifying terminal states and generating successor states for each game state.

2. Creating an evaluation function for the game states.

3. Implementing the search algorithm.

4. Integration: Accepting the current state from the judge, converting it into our own state representation, running the search algorithm to find the next move to play and outputting it in the desired format.



Alpha Beta Search is typically used for two-player competitive games and is a variant of naive Minimax search. It will be used for caluculating the evaluation function. There will be two functions for each bot and they will be different. The goal is to see wich one will act better and be more successful.
