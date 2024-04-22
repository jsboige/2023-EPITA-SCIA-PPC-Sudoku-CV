# Résolveur de Sudoku en utilisant Z3 avec Linq

## Auteurs

Mathieu LATOURNERIE\
Denis STOJILJKOVIC\
Tanguy MALANDAIN

#
## Introduction

Ce projet présente quelques solveurs de Sudoku en utilisant Z3 avec Linq.\
Voici les différentes méthodes utilisées pour résoudre le Sudoku:
- LinqToZ3Array
- LinqToZ3Grid 

Chaque méthode possède un fichier pour la résolution d'un Sudoku.

#
### LinqToZ3Array
Ce solveur possède un tableau de 81 variables de int, chaque variable représente une case du Sudoku. La première variable du tableau représente la case en haut à gauche du Sudoku, la dernière variable du tableau représente la case en bas à droite du Sudoku.

#
### LinqToZ3Grid
Ce solveur possède un tableau de 9 tableaux de 9 variables de int, chaque variable représente une case du Sudoku. Le premier index du tableau représente la ligne du Sudoku, le deuxième index du tableau représente la colonne du Sudoku.
