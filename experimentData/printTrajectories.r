course <- "Course2"
condition <- "VF"

# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/track_sphere/")

# load the necessary libraries
library("rgl")
library(geometry)
library(misc3d)

# set the output file
sink("log.out")

open3d()
# get vector of filenames
filenames = list.files(pattern="*.csv")

i <- 1
for(name in filenames){
	# retrieve list
	res = strsplit(filenames[i], "_")
	splitted = res[[1]]
	if(splitted[6]==condition && splitted[5]==course){		
		data = read.csv(header=T, sep=",", file=name)
		#print(name)
		if(i==1){
			
			#ts.surf1 <- t(convhulln(ps1))
			#convex1 <-  rgl.triangles(ps1[ts.surf1,1],ps1[ts.surf1,2],ps1[ts.surf1,3],col="gold2",alpha=.6)
			#print(ps1)			
			plot3d(data$x_pos,data$y_pos,data$z_pos)
		}	
		else{
			if(i==2) {
				mat <- matrix(c(data$x_pos,data$y_pos,data$z_pos), ncol=3)			
				print(mat)				
			}
			#points3d(data$x_pos,data$y_pos,data$z_pos)
		}	
	}
	i <- i+1
}



# load the dataset
#data = read.csv(header=T, sep=",", file="targetObject_graspedObject_0_0_Course1_0.csv")
#plot3d(data$x_pos,data$y_pos,data$z_pos)
#print(labels(data))

# close the output file
sink()

# unload the libraries
detach("package:rgl")
detach("package:geometry")
detach("package:misc3d")

# change back to the original directory
setwd(initial.dir)