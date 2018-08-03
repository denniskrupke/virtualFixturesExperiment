course <- "Course2"
condition <- "VF"

# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/")

# load the necessary libraries
#library("rgl")
library("pracma")

# set the output file
sink("log.out")

#open3d()
# get vector of filenames

data = read.csv(header=T, sep=",", file="main.csv")	
print(labels(data))
#plot(data$x_pos,data$y_pos,data$z_pos)

c1 <- data[which(data$course == "Course1"),]		
c1vf <- data[which(data$course == "Course1_VF"),]
c2 <- data[which(data$course == "Course2"),]		
c2vf <- data[which(data$course == "Course2_VF"),]		
c3 <- data[which(data$course == "Course3"),]		
c3vf <- data[which(data$course == "Course3_VF"),]

d <- density(c3vf$duration)
#plot(d)

print("mean durations: c1, c1vf, c2, ...")
print(mean(c1$duration))
print(mean(c1vf$duration))
print(mean(c2$duration))
print(mean(c2vf$duration))
print(mean(c3$duration))
print(mean(c3vf$duration))
print("")

print("mean collisions with gripper: c1, c1vf, c2, ...")
print(mean(c1$gripper_coll))
print(mean(c1vf$gripper_coll))
print(mean(c2$gripper_coll))
print(mean(c2vf$gripper_coll))
print(mean(c3$gripper_coll))
print(mean(c3vf$gripper_coll))
print("")

print("mean collisions with sphere: c1, c1vf, c2, ...")
print(mean(c1$obj_coll))
print(mean(c1vf$obj_coll))
print(mean(c2$obj_coll))
print(mean(c2vf$obj_coll))
print(mean(c3$obj_coll))
print(mean(c3vf$obj_coll))
print("")

outlier_c1 <- boxplot.stats(c1$duration)$out
outlier_c1vf <- boxplot.stats(c1vf$duration)$out
outlier_c2 <- boxplot.stats(c2$duration)$out
outlier_c2vf <- boxplot.stats(c2vf$duration)$out
outlier_c3 <- boxplot.stats(c3$duration)$out
outlier_c3vf <- boxplot.stats(c3vf$duration)$out

print("outlier durations")
print(outlier_c1)
print(outlier_c1vf)
print(outlier_c2)
print(outlier_c2vf)
print(outlier_c3)
print(outlier_c3vf)
print("")

c1_duration_clean <- c1$duration[!c1$duration %in% outlier_c1]
c1vf_duration_clean <- c1vf$duration[!c1vf$duration %in% outlier_c1vf]
c2_duration_clean <- c2$duration[!c2$duration %in% outlier_c2]
c2vf_duration_clean <- c2vf$duration[!c2vf$duration %in% outlier_c2vf]
c3_duration_clean <- c3$duration[!c3$duration %in% outlier_c3]
c3vf_duration_clean <- c3vf$duration[!c3vf$duration %in% outlier_c3vf]



VectorLength <- function(x){
	length <- sqrt(x[1]*x[1]+x[2]*x[2]+x[3]*x[3])

	return(length)
}

AngleDiff <- function(x,y){
	d <- dot(x,y)
	angle_diff <- acos(d/(VectorLength(x)*VectorLength(y)))

	return(angle_diff)
}

RadToDeg <- function(rad){
	deg <- rad*180/pi

	return(deg)
}

#print(VectorLength(c(0,1,1)))
print(RadToDeg(AngleDiff(c(1,0,0),c(0,1,0))))

# load the dataset
#data = read.csv(header=T, sep=",", file="targetObject_graspedObject_0_0_Course1_0.csv")
#plot3d(data$x_pos,data$y_pos,data$z_pos)
#print(labels(data))

# close the output file
sink()

# unload the libraries
#detach("package:rgl")

# change back to the original directory
#setwd(initial.dir)