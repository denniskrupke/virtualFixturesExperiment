course <- "Course1"
condition <- "VF"

courses <- c("Course1", "Course2", "Course3")

# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/track_head")

# load the necessary libraries
library("rgl")
library("pracma")
library("BayesFactor")

# set the output file
sink("head_movements.out")

#open3d()
# get vector of filenames

#data = read.csv(header=T, sep=",", file="main.csv")	
#print(labels(data))
#plot(data$x_pos,data$y_pos,data$z_pos)

# c1 <- data[which(data$course == "Course1"),]		
# c1vf <- data[which(data$course == "Course1_VF"),]
# c2 <- data[which(data$course == "Course2"),]		
# c2vf <- data[which(data$course == "Course2_VF"),]		
# c3 <- data[which(data$course == "Course3"),]		
# c3vf <- data[which(data$course == "Course3_VF"),]

# d <- density(c3vf$duration)
#plot(d)

# print("mean durations: c1, c1vf, c2, ...")
# print(mean(c1$duration))
# print(mean(c1vf$duration))
# print(mean(c2$duration))
# print(mean(c2vf$duration))
# print(mean(c3$duration))
# print(mean(c3vf$duration))
# print("")

# print("mean collisions with gripper: c1, c1vf, c2, ...")
# print(mean(c1$gripper_coll))
# print(mean(c1vf$gripper_coll))
# print(mean(c2$gripper_coll))
# print(mean(c2vf$gripper_coll))
# print(mean(c3$gripper_coll))
# print(mean(c3vf$gripper_coll))
# print("")

# print("mean collisions with sphere: c1, c1vf, c2, ...")
# print(mean(c1$obj_coll))
# print(mean(c1vf$obj_coll))
# print(mean(c2$obj_coll))
# print(mean(c2vf$obj_coll))
# print(mean(c3$obj_coll))
# print(mean(c3vf$obj_coll))
# print("")

# outlier_c1 <- boxplot.stats(c1$duration)$out
# outlier_c1vf <- boxplot.stats(c1vf$duration)$out
# outlier_c2 <- boxplot.stats(c2$duration)$out
# outlier_c2vf <- boxplot.stats(c2vf$duration)$out
# outlier_c3 <- boxplot.stats(c3$duration)$out
# outlier_c3vf <- boxplot.stats(c3vf$duration)$out

# print("outlier durations")
# print(outlier_c1)
# print(outlier_c1vf)
# print(outlier_c2)
# print(outlier_c2vf)
# print(outlier_c3)
# print(outlier_c3vf)
# print("")

# c1_duration_clean <- c1$duration[!c1$duration %in% outlier_c1]
# c1vf_duration_clean <- c1vf$duration[!c1vf$duration %in% outlier_c1vf]
# c2_duration_clean <- c2$duration[!c2$duration %in% outlier_c2]
# c2vf_duration_clean <- c2vf$duration[!c2vf$duration %in% outlier_c2vf]
# c3_duration_clean <- c3$duration[!c3$duration %in% outlier_c3]
# c3vf_duration_clean <- c3vf$duration[!c3vf$duration %in% outlier_c3vf]



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


# get vector of filenames
filenames = list.files(pattern="*.csv")

angleSum <- 0
distSum <- 0
angleSum_vf <- 0
distSum_vf <- 0

lastForward <- 0
lastPos <- 0 
currentForward <- 0
currentPos <- 0 
summedAngles <- c()
summedDistances <- c()
summedAngles_vf <- c()
summedDistances_vf <- c()


#settings for the plot
colNoVF <- "grey77"
colVF <- "grey44"
wdth <- 5
hght <- 7

c1_rot <- c()
c1_vf_rot <- c()
c2_rot <- c()
c2_vf_rot <- c()
c3_rot <- c()
c3_vf_rot <- c()

c1_trans <- c()
c1_vf_trans <- c()
c2_trans <- c()
c2_vf_trans <- c()
c3_trans <- c()
c3_vf_trans <- c()

for(c in courses){	
	i <- 1
	for(name in filenames){
		# retrieve list
		res = strsplit(filenames[i], "_")
		splitted = res[[1]]
		if(splitted[6]!=condition && splitted[5]==c){		# no VF
			tmp = read.csv(header=T, sep=",", file=name)
			for(index in 2:length(tmp$forward_x)){
				#angles-sum
				lastForward <- c(tmp$forward_x[index-1], tmp$forward_y[index-1], tmp$forward_z[index-1])
				currentForward <- c(tmp$forward_x[index], tmp$forward_y[index], tmp$forward_z[index])
				angleSum <- angleSum + RadToDeg(AngleDiff(lastForward, currentForward))
				#euclid-dist
				lastPos <- c(tmp$x_pos[index-1], tmp$y_pos[index-1], tmp$z_pos[index-1])
				currentPos <- c(tmp$x_pos[index], tmp$y_pos[index], tmp$z_pos[index])
				distSum <- distSum + dist(rbind(lastPos,currentPos))
			}
			summedAngles <- c(summedAngles,angleSum)
			summedDistances <- c(summedDistances, distSum)					

			if(c=="Course1"){
				c1_rot <- c(c1_rot, angleSum)
				c1_trans <- c(c1_trans, distSum)
			}					
			else if(c=="Course2"){
				c2_rot <- c(c2_rot, angleSum)
				c2_trans <- c(c2_trans, distSum)
			}					
			else if(c=="Course3"){
				c3_rot <- c(c3_rot, angleSum)
				c3_trans <- c(c3_trans, distSum)
			}	

			angleSum <- 0
			distSum <- 0				
		}
		else if(splitted[6]==condition && splitted[5]==c){		# VF
			tmp = read.csv(header=T, sep=",", file=name)
			for(index in 2:length(tmp$forward_x)){
				#angles-sum
				lastForward <- c(tmp$forward_x[index-1], tmp$forward_y[index-1], tmp$forward_z[index-1])
				currentForward <- c(tmp$forward_x[index], tmp$forward_y[index], tmp$forward_z[index])
				angleSum_vf <- angleSum_vf + RadToDeg(AngleDiff(lastForward, currentForward))
				#euclid-dist
				lastPos <- c(tmp$x_pos[index-1], tmp$y_pos[index-1], tmp$z_pos[index-1])
				currentPos <- c(tmp$x_pos[index], tmp$y_pos[index], tmp$z_pos[index])
				distSum_vf <- distSum_vf + dist(rbind(lastPos,currentPos))
			}
			summedAngles_vf <- c(summedAngles_vf,angleSum_vf)
			summedDistances_vf <- c(summedDistances_vf, distSum_vf)					

			if(c=="Course1"){
				c1_vf_rot <- c(c1_vf_rot, angleSum_vf)
				c1_vf_trans <- c(c1_vf_trans, distSum_vf)
			}					
			else if(c=="Course2"){
				c2_vf_rot <- c(c2_vf_rot, angleSum_vf)
				c2_vf_trans <- c(c2_vf_trans, distSum_vf)
			}					
			else if(c=="Course3"){
				c3_vf_rot <- c(c3_vf_rot, angleSum_vf)
				c3_vf_trans <- c(c3_vf_trans, distSum_vf)
			}	
			angleSum_vf <- 0
			distSum_vf <- 0				
		}		
		i <- i+1
	}
}

	print("------------------Check normality on data without outliers---------------------")
	print("Shapiro-Wilk Test")
	print("c1 rot")
	print(shapiro.test(c1_rot))
	print("c1vf rot")
	print(shapiro.test(c1_vf_rot))
	print("c2 rot")
	print(shapiro.test(c2_rot))
	print("c2vf rot")
	print(shapiro.test(c2_vf_rot))
	print("c3 rot")
	print(shapiro.test(c3_rot))
	print("c3vf rot")
	print(shapiro.test(c3_vf_rot))

	print("c1 trans")
	print(shapiro.test(c1_trans))
	print("c1vf trans")
	print(shapiro.test(c1_vf_trans))
	print("c2 trans")
	print(shapiro.test(c2_trans))
	print("c2vf trans")
	print(shapiro.test(c2_vf_trans))
	print("c3 trans")
	print(shapiro.test(c3_trans))
	print("c3vf trans")
	print(shapiro.test(c3_vf_trans))


	print("lengths")
	print(length(c1_rot))
	print(length(c1_vf_rot))
	print(length(c2_rot))
	print(length(c2_vf_rot))
	print(length(c3_rot))
	print(length(c3_vf_rot))



	print("------------------------Check significance/p-values on raw data---------------------------")
	print("c1 rot")
	print(wilcox.test(c1_rot, c1_vf_rot, paired=TRUE))
	print("c2 rot")
	print(wilcox.test(c2_rot, c2_vf_rot, paired=TRUE))
	print("c3 rot")
	print(wilcox.test(c3_rot, c3_vf_rot, paired=TRUE))

	print("--------------------------------------------------------------------")
	print("c1 trans")
	print(wilcox.test(c1_trans, c1_vf_trans, paired=TRUE))
	print("c2 trans")
	print(wilcox.test(c2_trans, c2_vf_trans, paired=TRUE))
	print("c3 trans")
	print(wilcox.test(c3_trans, c3_vf_trans, paired=TRUE))


	print("------------------Check Baysian Factor of means---------------------")
	print("c1 rot")
	print(ttestBF(x=c1_rot, y=c1_vf_rot, paired=TRUE))
	print("c2 rot")
	print(ttestBF(x=c2_rot, y=c2_vf_rot, paired=TRUE))
	print("c3 rot")
	print(ttestBF(x=c3_rot, y=c3_vf_rot, paired=TRUE))

	print("--------------------------------------------------------------------")
	print("c1 trans")
	print(ttestBF(x=c1_trans, y=c1_vf_trans, paired=TRUE))
	print("c2 trans")
	print(ttestBF(x=c2_trans, y=c2_vf_trans, paired=TRUE))
	print("c3 trans")
	print(ttestBF(x=c3_trans, y=c3_vf_trans, paired=TRUE))


	pdf("summary_rotations.pdf", width=2*wdth, height=hght)
	boxplot(c1_rot, c1_vf_rot, c2_rot, c2_vf_rot, c3_rot, c3_vf_rot, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head rotations in degree')
	dev.off()

	pdf("summary_translations.pdf", width=2*wdth, height=hght)
	boxplot(c1_trans, c1_vf_trans, c2_trans, c2_vf_trans, c3_trans, c3_vf_trans, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head translations in meter')
	dev.off()
	

	# pdf(paste("head_rotations_",c,".pdf"), width=wdth, height=hght)
	# outlier <- boxplot.stats(summedAngles)$out
	# clean <- summedAngles[!summedAngles %in% outlier]
	# outlier_vf <- boxplot.stats(summedAngles_vf)$out
	# clean_vf <- summedAngles_vf[!summedAngles_vf %in% outlier_vf]
	# boxplot(clean, clean_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='summed head rotations in degree', main=c)
	# dev.off()

	# pdf(paste("head_translations_",c,".pdf"), width=wdth, height=hght)
	# outlier <- boxplot.stats(summedDistances)$out
	# clean <- summedDistances[!summedDistances %in% outlier]
	# outlier_vf <- boxplot.stats(summedDistances_vf)$out
	# clean_vf <- summedDistances_vf[!summedDistances_vf %in% outlier_vf]
	# boxplot(clean, clean_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='summed head translations in meter', main=c)
	# dev.off()

	c1_rot <- c1_rot[!c1_rot %in% boxplot.stats(c1_rot)$out]
	c1_vf_rot <- c1_vf_rot[!c1_vf_rot %in% boxplot.stats(c1_vf_rot)$out]
	c2_rot <- c2_rot[!c2_rot %in% boxplot.stats(c2_rot)$out]
	c2_vf_rot <- c2_vf_rot[!c2_vf_rot %in% boxplot.stats(c2_vf_rot)$out]
	c3_rot <- c3_rot[!c3_rot %in% boxplot.stats(c3_rot)$out]
	c3_vf_rot <- c3_vf_rot[!c3_vf_rot %in% boxplot.stats(c3_vf_rot)$out]

	c1_trans <- c1_trans[!c1_trans %in% boxplot.stats(c1_trans)$out]
	c1_vf_trans <- c1_vf_trans[!c1_vf_trans %in% boxplot.stats(c1_vf_trans)$out]
	c2_trans <- c2_trans[!c2_trans %in% boxplot.stats(c2_trans)$out]
	c2_vf_trans <- c2_vf_trans[!c2_vf_trans %in% boxplot.stats(c2_vf_trans)$out]
	c3_trans <- c3_trans[!c3_trans %in% boxplot.stats(c3_trans)$out]
	c3_vf_trans <- c3_vf_trans[!c3_vf_trans %in% boxplot.stats(c3_vf_trans)$out]
	
	pdf("summary_rotations_clean.pdf", width=2*wdth, height=hght)
	boxplot(c1_rot, c1_vf_rot, c2_rot, c2_vf_rot, c3_rot, c3_vf_rot, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head rotations in degree')
	dev.off()

	pdf("summary_translations_clean.pdf", width=2*wdth, height=hght)
	boxplot(c1_trans, c1_vf_trans, c2_trans, c2_vf_trans, c3_trans, c3_vf_trans, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head translations in meter')
	dev.off()

	

	sink()
