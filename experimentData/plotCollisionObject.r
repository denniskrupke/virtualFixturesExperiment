# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/")

# load the necessary libraries
#library("rgl")
library("pracma")
library("BayesFactor")

# set the output file
sink("main_coll_object.out")

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


print("mean collisions with sphere: c1, c1vf, c2, ...")
print(mean(c1$obj_coll))
print(mean(c1vf$obj_coll))
print(mean(c2$obj_coll))
print(mean(c2vf$obj_coll))
print(mean(c3$obj_coll))
print(mean(c3vf$obj_coll))
print("")

#outlier_c1 <- boxplot.stats(c1$obj_coll)$out
#outlier_c1vf <- boxplot.stats(c1vf$obj_coll)$out
#outlier_c2 <- boxplot.stats(c2$obj_coll)$out
#outlier_c2vf <- boxplot.stats(c2vf$obj_coll)$out
#outlier_c3 <- boxplot.stats(c3$obj_coll)$out
#outlier_c3vf <- boxplot.stats(c3vf$obj_coll)$out

#print("outlier durations")
#print(outlier_c1)
#print(outlier_c1vf)
#print(outlier_c2)
#print(outlier_c2vf)
#print(outlier_c3)
#print(outlier_c3vf)
#print("")

c1_obj_coll_clean <- c1$obj_coll#c1$obj_coll[!c1$obj_coll %in% outlier_c1]
c1vf_obj_coll_clean <- c1vf$obj_coll#c1vf$obj_coll[!c1vf$obj_coll %in% outlier_c1vf]
c2_obj_coll_clean <- c2$obj_coll#c2$obj_coll[!c2$obj_coll %in% outlier_c2]
c2vf_obj_coll_clean <- c2vf$obj_coll#c2vf$obj_coll[!c2vf$obj_coll %in% outlier_c2vf]
c3_obj_coll_clean <- c3$obj_coll#c3$obj_coll[!c3$obj_coll %in% outlier_c3]
c3vf_obj_coll_clean <- c3vf$obj_coll#c3vf$obj_coll[!c3vf$obj_coll %in% outlier_c3vf]

print("------------------Check normality on data without outliers---------------------")
print("c1")
#print(shapiro.test(c1_obj_coll_clean))
print("c1vf")
#print(shapiro.test(c1vf_obj_coll_clean))

print("c2")
print(shapiro.test(c2_obj_coll_clean))
print("c2vf")
print(shapiro.test(c2vf_obj_coll_clean))

print("c3")
print(shapiro.test(c3_obj_coll_clean))
print("c3vf")
print(shapiro.test(c3vf_obj_coll_clean))


print("------------------Check significance---------------------")
print("Wilcox Test")

print("c1")
print(wilcox.test(c1_obj_coll_clean, c1vf_obj_coll_clean, paired=TRUE))

print("c2")
print(wilcox.test(c2_obj_coll_clean, c2vf_obj_coll_clean, paired=TRUE))

print("c3")
print(wilcox.test(c3_obj_coll_clean, c3vf_obj_coll_clean, paired=TRUE))

print("------------------Check Baysian Factor of means---------------------")
print("c1")
print(ttestBF(x=c1_obj_coll_clean, y=c1vf_obj_coll_clean, paired=TRUE))
print("c2")
print(ttestBF(x=c2_obj_coll_clean, y=c2vf_obj_coll_clean, paired=TRUE))
print("c3")
print(ttestBF(x=c3_obj_coll_clean, y=c3vf_obj_coll_clean, paired=TRUE))

# close the output file
sink()


colNoVF <- "grey77"
colVF <- "grey44"

wdth <- 5
hght <- 7

pdf("c1_obj_coll.pdf", width=wdth, height=hght)
boxplot(c1_obj_coll_clean, c1vf_obj_coll_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='collisions per trial', main='Course 1')
dev.off()

pdf("c2_obj_coll.pdf", width=wdth, height=hght)
boxplot(c2_obj_coll_clean, c2vf_obj_coll_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='collisions per trial', main='Course 2')
dev.off()

pdf("c3_obj_coll.pdf", width=wdth, height=hght)
boxplot(c3_obj_coll_clean, c3vf_obj_coll_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='collisions per trial', main='Course 3')
dev.off()

pdf("summary_obj_coll_title.pdf", width=2*wdth, height=hght)
boxplot(c1_obj_coll_clean, c1vf_obj_coll_clean,c2_obj_coll_clean, c2vf_obj_coll_clean, c3_obj_coll_clean, c3vf_obj_coll_clean, names=c('C1 without VF','C1 VF','C2 without VF','C2 VF','C3 without VF','C3 VF'), col=c(colNoVF,colVF), ylab='collisions per trial', main='Summary object collisions')
dev.off()

pdf("summary_obj_coll.pdf", width=2*wdth, height=hght)
boxplot(c1_obj_coll_clean, c1vf_obj_coll_clean,c2_obj_coll_clean, c2vf_obj_coll_clean, c3_obj_coll_clean, c3vf_obj_coll_clean, names=c('C1 without VF','C1 VF','C2 without VF','C2 VF','C3 without VF','C3 VF'), col=c(colNoVF,colVF), ylab='collisions per trial')
dev.off()

pdf("means_obj_coll.pdf", width=2*wdth, height=hght)
plot(c(mean(c1_obj_coll_clean), mean(c1vf_obj_coll_clean),
	mean(c2_obj_coll_clean), mean(c2vf_obj_coll_clean),
	mean(c3_obj_coll_clean), mean(c3vf_obj_coll_clean)),		
	ylab='mean collisions per trial',
	xlab='course',
	xaxt="n")
axis(1, at=1:6, labels=c('C1 without VF','C1 with VF','C2 without VF','C2 with VF','C3 without VF','C3 with VF'))
dev.off()

#c1_obj_coll_clean <- c1$obj_coll[!c1$obj_coll %in% outlier_c1]
#c1vf_obj_coll_clean <- c1vf$obj_coll[!c1vf$obj_coll %in% outlier_c1vf]
#c2_obj_coll_clean <- c2$obj_coll[!c2$obj_coll %in% outlier_c2]
#c2vf_obj_coll_clean <- c2vf$obj_coll[!c2vf$obj_coll %in% outlier_c2vf]
#c3_obj_coll_clean <- c3$obj_coll[!c3$obj_coll %in% outlier_c3]
#c3vf_obj_coll_clean <- c3vf$obj_coll[!c3vf$obj_coll %in% outlier_c3vf]


# unload the libraries
#detach("package:rgl")

# change back to the original directory
#setwd(initial.dir)