
# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/precision")

# load the necessary libraries
library("rgl")
library("pracma")
library("BayesFactor")

# set the output file
sink("precision.out")


#settings for the plot
colNoVF <- "grey77"
colVF <- "grey44"
wdth <- 5
hght <- 7

c1 <- c()
c1_vf <- c()
c2 <- c()
c2_vf <- c()
c3 <- c()
c3_vf <- c()

c1_means <- c()
c1_vf_means <- c()
c2_means <- c()
c2_vf_means <- c()
c3_means <- c()
c3_vf_means <- c()

c1_min <- c()
c1_vf_min <- c()
c2_min <- c()
c2_vf_min <- c()
c3_min <- c()
c3_vf_min <- c()

c1_count <- 0
c2_count <- 0
c3_count <- 0
c1_vf_count <- 0
c2_vf_count <- 0
c3_vf_count <- 0

# get vector of filenames
filenames = list.files(pattern="*.csv")

for(name in filenames){
	tmp = read.csv(header=T, sep=",", file=name)
	# retrieve list
	res = strsplit(name, "_")
	splitted = res[[1]]
	if(splitted[5]!="VF"){
		if(splitted[4] == "Course1"){
			c1 <- c(c1, tmp$shortestDistance)
			c1_means <- c(c1_means, mean(tmp$shortestDistance))
			c1_min <- c(c1_min, min(tmp$shortestDistance))
			c1_count <- c1_count+1
		}
		else if(splitted[4] == "Course2"){
			c2 <- c(c2, tmp$shortestDistance)
			c2_means <- c(c2_means, mean(tmp$shortestDistance))
			c2_min <- c(c2_min, min(tmp$shortestDistance))
			c2_count <- c2_count+1
		}
		else if(splitted[4] == "Course3"){
			c3 <- c(c3, tmp$shortestDistance)
			c3_means <- c(c3_means, mean(tmp$shortestDistance))
			c3_min <- c(c3_min, min(tmp$shortestDistance))
			c3_count <- c3_count+1
		}
	}
	else{
		if(splitted[4] == "Course1"){
			c1_vf <- c(c1_vf, tmp$shortestDistance)
			c1_vf_means <- c(c1_vf_means, mean(tmp$shortestDistance))
			c1_vf_min <- c(c1_vf_min, min(tmp$shortestDistance))
			c1_vf_count <- c1_vf_count+1
		}
		else if(splitted[4] == "Course2"){
			c2_vf <- c(c2_vf, tmp$shortestDistance)
			c2_vf_means <- c(c2_vf_means, mean(tmp$shortestDistance))
			c2_vf_min <- c(c2_vf_min, min(tmp$shortestDistance))
			c2_vf_count <- c2_vf_count+1
		}
		else if(splitted[4] == "Course3"){
			c3_vf <- c(c3_vf, tmp$shortestDistance)
			c3_vf_means <- c(c3_vf_means, mean(tmp$shortestDistance))
			c3_vf_min <- c(c3_vf_min, min(tmp$shortestDistance))
			c3_vf_count <- c3_vf_count+1
		}
	}
}

	print("number of files")
	print(c1_count)
	print(c1_vf_count)
	print(c2_count)
	print(c2_vf_count)
	print(c3_count)
	print(c3_vf_count)

	pdf("precision_c1.pdf", width=wdth, height=hght)
	boxplot(c1, c1_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_c2.pdf", width=wdth, height=hght)
	boxplot(c2, c2_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_c3.pdf", width=wdth, height=hght)
	boxplot(c3, c3_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_summary.pdf", width=2*wdth, height=hght)
	boxplot(c1, c1_vf, c2, c2_vf, c3, c3_vf, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF,colNoVF,colVF,colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()


	print("------------------Check normality on data means with outliers---------------------")
	print("Shapiro-Wilk Test means")
	print("c1")
	print(shapiro.test(c1_means))
	print("c1vf")
	print(shapiro.test(c1_vf_means))
	print("c2")
	print(shapiro.test(c2_means))
	print("c2vf")
	print(shapiro.test(c2_vf_means))
	print("c3")
	print(shapiro.test(c3_means))
	print("c3vf")
	print(shapiro.test(c3_vf_means))

	print("------------------Check normality on data mins with outliers---------------------")
	print("Shapiro-Wilk Test mins")
	print("c1")
	print(shapiro.test(c1_min))
	print("c1vf")
	print(shapiro.test(c1_vf_min))
	print("c2")
	print(shapiro.test(c2_min))
	print("c2vf")
	print(shapiro.test(c2_vf_min))
	print("c3")
	print(shapiro.test(c3_min))
	print("c3vf")
	print(shapiro.test(c3_vf_min))


	print("------------------Check significance of means---------------------")
	print("Wilcox Test: means")
	print("c1")
	print(wilcox.test(c1_means,c1_vf_means,paired=TRUE))
	print("c2")
	print(wilcox.test(c2_means,c2_vf_means,paired=TRUE))
	print("c3")
	print(wilcox.test(c3_means,c3_vf_means,paired=TRUE))

	print("------------------Check significance of mins---------------------")
	print("Wilcox Test: mins")
	print("c1")
	print(wilcox.test(c1_min,c1_vf_min,paired=TRUE))
	print("c2")
	print(wilcox.test(c2_min,c2_vf_min,paired=TRUE))
	print("c3")
	print(wilcox.test(c3_min,c3_vf_min,paired=TRUE))


	print("------------------Check Baysian Factor of means---------------------")
	print("c1 mean")
	print(ttestBF(x=c1_means, y=c1_vf_means, paired=TRUE))
	print("c2 mean")
	print(ttestBF(x=c2_means, y=c2_vf_means, paired=TRUE))
	print("c3 mean")
	print(ttestBF(x=c3_means, y=c3_vf_means, paired=TRUE))

	print("------------------Check Baysian Factor of mins---------------------")
	print("c1 min")
	print(ttestBF(x=c1_min, y=c1_vf_min, paired=TRUE))
	print("c2 min")
	print(ttestBF(x=c2_min, y=c2_vf_min, paired=TRUE))
	print("c3 min")
	print(ttestBF(x=c3_min, y=c3_vf_min, paired=TRUE))


	pdf("precision_mean_summary.pdf", width=2*wdth, height=hght)
	boxplot(c1_means, c1_vf_means, c2_means, c2_vf_means, c3_means, c3_vf_means, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF,colNoVF,colVF,colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	# ---------------removing outliers-----------------------------------------
	c1 <- c1[!c1 %in% boxplot.stats(c1)$out]
	c1_vf <- c1_vf[!c1_vf %in% boxplot.stats(c1_vf)$out]
	c2 <- c2[!c2 %in% boxplot.stats(c2)$out]
	c2_vf <- c2_vf[!c2_vf %in% boxplot.stats(c2_vf)$out]
	c3 <- c3[!c3 %in% boxplot.stats(c3)$out]
	c3_vf <- c3_vf[!c3_vf %in% boxplot.stats(c3_vf)$out]

	c1_means <- c1_means[!c1_means %in% boxplot.stats(c1_means)$out]
	c1_vf_means <- c1_vf_means[!c1_vf_means %in% boxplot.stats(c1_vf_means)$out]
	c2_means <- c2_means[!c2_means %in% boxplot.stats(c2_means)$out]
	c2_vf_means <- c2_vf_means[!c2_vf_means %in% boxplot.stats(c2_vf_means)$out]
	c3_means <- c3_means[!c3_means %in% boxplot.stats(c3_means)$out]
	c3_vf_means <- c3_vf_means[!c3_vf_means %in% boxplot.stats(c3_vf_means)$out]

	c1_min <- c1_min[!c1_min %in% boxplot.stats(c1_min)$out]
	c1_vf_min <- c1_vf_min[!c1_vf_min %in% boxplot.stats(c1_vf_min)$out]
	c2_min <- c2_min[!c2_min %in% boxplot.stats(c2_min)$out]
	c2_vf_min <- c2_vf_min[!c2_vf_min %in% boxplot.stats(c2_vf_min)$out]
	c3_min <- c3_min[!c3_min %in% boxplot.stats(c3_min)$out]
	c3_vf_min <- c3_vf_min[!c3_vf_min %in% boxplot.stats(c3_vf_min)$out]



	pdf("precision_c1_clean.pdf", width=wdth, height=hght)
	boxplot(c1, c1_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_c2_clean.pdf", width=wdth, height=hght)
	boxplot(c2, c2_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_c3_clean.pdf", width=wdth, height=hght)
	boxplot(c3, c3_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_summary_clean.pdf", width=2*wdth, height=hght)
	boxplot(c1, c1_vf, c2, c2_vf, c3, c3_vf, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF,colNoVF,colVF,colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()	

	pdf("precision_min_summary_clean.pdf", width=2*wdth, height=hght)
	boxplot(c1_min, c1_vf_min, c2_min, c2_vf_min, c3_min, c3_vf_min, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF,colNoVF,colVF,colNoVF,colVF), ylab='distance in meters')#, main=c)
	dev.off()

	pdf("precision_mean_summary_clean.pdf", width=2*wdth, height=hght)
	boxplot(c1_means, c1_vf_means, c2_means, c2_vf_means, c3_means, c3_vf_means, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF,colNoVF,colVF,colNoVF,colVF), ylab='distance in meters')#, main=c)
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

	# c1_rot <- c1_rot[!c1_rot %in% boxplot.stats(c1_rot)$out]
	# c1_vf_rot <- c1_vf_rot[!c1_vf_rot %in% boxplot.stats(c1_vf_rot)$out]
	# c2_rot <- c2_rot[!c2_rot %in% boxplot.stats(c2_rot)$out]
	# c2_vf_rot <- c2_vf_rot[!c2_vf_rot %in% boxplot.stats(c2_vf_rot)$out]
	# c3_rot <- c3_rot[!c3_rot %in% boxplot.stats(c3_rot)$out]
	# c3_vf_rot <- c3_vf_rot[!c3_vf_rot %in% boxplot.stats(c3_vf_rot)$out]

	# c1_trans <- c1_trans[!c1_trans %in% boxplot.stats(c1_trans)$out]
	# c1_vf_trans <- c1_vf_trans[!c1_vf_trans %in% boxplot.stats(c1_vf_trans)$out]
	# c2_trans <- c2_trans[!c2_trans %in% boxplot.stats(c2_trans)$out]
	# c2_vf_trans <- c2_vf_trans[!c2_vf_trans %in% boxplot.stats(c2_vf_trans)$out]
	# c3_trans <- c3_trans[!c3_trans %in% boxplot.stats(c3_trans)$out]
	# c3_vf_trans <- c3_vf_trans[!c3_vf_trans %in% boxplot.stats(c3_vf_trans)$out]
	
	# pdf("summary_rotations.pdf", width=2*wdth, height=hght)
	# boxplot(c1_rot, c1_vf_rot, c2_rot, c2_vf_rot, c3_rot, c3_vf_rot, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head rotations in degree')
	# dev.off()

	# pdf("summary_translations.pdf", width=2*wdth, height=hght)
	# boxplot(c1_trans, c1_vf_trans, c2_trans, c2_vf_trans, c3_trans, c3_vf_trans, names=c('C1 without VF','C1 with VF', 'C2 without VF','C2 with VF', 'C3 without VF','C3 with VF'), col=c(colNoVF,colVF, colNoVF,colVF, colNoVF,colVF), ylab='Head translations in meter')
	# dev.off()


	# print("------------------Check normality on data without outliers---------------------")
	# print("Shapiro-Wilk Test")
	# print("c1 rot")
	# print(shapiro.test(c1_rot))
	# print("c1vf rot")
	# print(shapiro.test(c1_vf_rot))
	# print("c2 rot")
	# print(shapiro.test(c2_rot))
	# print("c2vf rot")
	# print(shapiro.test(c2_vf_rot))
	# print("c3 rot")
	# print(shapiro.test(c3_rot))
	# print("c3vf rot")
	# print(shapiro.test(c3_vf_rot))

	# print("c1 trans")
	# print(shapiro.test(c1_trans))
	# print("c1vf trans")
	# print(shapiro.test(c1_vf_trans))
	# print("c2 trans")
	# print(shapiro.test(c2_trans))
	# print("c2vf trans")
	# print(shapiro.test(c2_vf_trans))
	# print("c3 trans")
	# print(shapiro.test(c3_trans))
	# print("c3vf trans")
	# print(shapiro.test(c3_vf_trans))


	# print("------------------------Check significance/p-values on clean data---------------------------")
	# print("c1 rot")
	# print(wilcox.test(c1_rot, c1_vf_rot))
	# print("c2 rot")
	# print(wilcox.test(c2_rot, c2_vf_rot))
	# print("c3 rot")
	# print(wilcox.test(c3_rot, c3_vf_rot))

	# print("c1 trans")
	# print(wilcox.test(c1_trans, c1_vf_trans))
	# print("c2 trans")
	# print(wilcox.test(c2_trans, c2_vf_trans))
	# print("c3 trans")
	# print(wilcox.test(c3_trans, c3_vf_trans))

	

	sink()
