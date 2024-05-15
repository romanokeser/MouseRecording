import torch
import numpy as np
import matplotlib.pyplot as plt
import argparse

# Set up argument parser
parser = argparse.ArgumentParser(description='Generate heatmap from point counts.')
parser.add_argument('file_path', type=str, help='Path to the point counts file')
args = parser.parse_args()

# Read the points from the file specified by the command-line argument
points_file_path = args.file_path
with open(points_file_path, 'r') as file:
    lines = file.readlines()
    point_counts = [[int(num) for num in line.split()] for line in lines]

# Set these values based on tracked mouse dots (0 - no activity, inf - max activity)
grid_values = np.array(point_counts)

grid_tensor = torch.tensor(grid_values, dtype=torch.float)
resized_grid = torch.nn.functional.interpolate(grid_tensor.unsqueeze(0).unsqueeze(0), size=(1080, 1920), mode='bicubic').squeeze()
resized_grid = resized_grid.numpy()

plt.figure(figsize=(16, 9)) 
plt.imshow(resized_grid, cmap='inferno', interpolation='nearest')
plt.colorbar()
plt.title('Interpolated Heatmap of Grid Values (1920x1080)')
plt.xlabel('X-axis')
plt.ylabel('Y-axis')
plt.show()
