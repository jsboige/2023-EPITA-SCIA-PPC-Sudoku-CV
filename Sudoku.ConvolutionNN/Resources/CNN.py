import copy
import tensorflow as tf
from tensorflow import keras
import numpy as np
import pandas as pd

from timeit import default_timer

model = keras.models.load_model(model_path)

def norm(a):    
    return (a / 9) - .5

def denorm(a):
    return (a + .5) * 9

def inference_sudoku(sample):
    
    '''
        This function solve the sudoku by filling blank positions one by one.
    '''
    
    feat = copy.copy(sample)
    
    while(1):
        out = model.predict(feat.reshape((1,9,9,1)))  
        out = out.squeeze()

        pred = np.argmax(out, axis=1).reshape((9,9))+1 
        prob = np.around(np.max(out, axis=1).reshape((9,9)), 2) 
        
        feat = denorm(feat).reshape((9,9))
        mask = (feat==0)
     
        if(mask.sum()==0):
            break
            
        prob_new = prob*mask
    
        ind = np.argmax(prob_new)
        x, y = (ind//9), (ind%9)

        val = pred[x][y]
        feat[x][y] = val
        feat = norm(feat)
    
    return pred

def solve_sudoku(game):
    game = np.array(game)
    game = norm(game)
    game = inference_sudoku(game)
    return game.tolist()

game = solve_sudoku(instance)

r=game