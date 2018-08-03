# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData")

# load the necessary libraries
#library(nlme)

# set the output file
sink("log.out")

# load the dataset
main_data = read.csv(header=T, sep=",", file="main.csv")
print(labels(main_data))

# close the output file
sink()

# unload the libraries
#detach("package:nlme")

# change back to the original directory
setwd(initial.dir)