#course <- "Course1"
#condition <- "VF"

# store the current directory
initial.dir<-getwd()

# change to the new directory
setwd("/Users/dennis/git/virtualFixtures/experimentData/")

# load the necessary libraries
library("rgl")
library("pracma")
library("BayesFactor")

# set the output file
sink("questionnaires.out")

colNoVF <- "grey77"
colVF <- "grey44"

wdth <- 5
hght <- 7

#open3d()
# get vector of filenames

data = read.csv(header=T, sep=",", file="questionnaires.csv")	

print("AttrakDiff pq mean:")
print(mean(data$attrakDiff2_pq))
print(mean(data$attrakDiff2_vf_pq))
print("AttrakDiff hq mean:")
print(mean(data$attrakDiff2_hq))
print(mean(data$attrakDiff2_vf_hq))

print("AttrakDiff pq sd:")
print(sd(data$attrakDiff2_pq))
print(sd(data$attrakDiff2_vf_pq))
print("AttrakDiff hq sd:")
print(sd(data$attrakDiff2_hq))
print(sd(data$attrakDiff2_vf_hq))

print("---pq--")
print(mean(data$attrakDiff2_pq) - sd(data$attrakDiff2_pq))
print(mean(data$attrakDiff2_pq) + sd(data$attrakDiff2_pq))
print("---pq-vf--")
print(mean(data$attrakDiff2_vf_pq) - sd(data$attrakDiff2_vf_pq))
print(mean(data$attrakDiff2_vf_pq) + sd(data$attrakDiff2_vf_pq))

print("---hq--")
print(mean(data$attrakDiff2_hq) - sd(data$attrakDiff2_hq))
print(mean(data$attrakDiff2_hq) + sd(data$attrakDiff2_hq))
print("---hq-vf--")
print(mean(data$attrakDiff2_vf_hq) - sd(data$attrakDiff2_vf_hq))
print(mean(data$attrakDiff2_vf_hq) + sd(data$attrakDiff2_vf_hq))


#------------------Determine outliers-------------------
print("-------------------Determine outliers-------------------------")
out_sus_pq <- boxplot.stats(data$sus_pq)$out
out_sus_pq_vf <- boxplot.stats(data$sus_pq_vf)$out

out_sus <- boxplot.stats(data$sus)$out
out_sus_vf <- boxplot.stats(data$sus_vf)$out

out_attrakDiff2_att <- boxplot.stats(data$attrakDiff2_att)$out
out_attrakDiff2_pq <- boxplot.stats(data$attrakDiff2_pq)$out
out_attrakDiff2_hqi <- boxplot.stats(data$attrakDiff2_hqi)$out
out_attrakDiff2_hqs <- boxplot.stats(data$attrakDiff2_hqs)$out
out_attrakDiff2_hq <- boxplot.stats(data$attrakDiff2_hq)$out
out_attrakDiff2_vf_att <- boxplot.stats(data$attrakDiff2_vf_att)$out
out_attrakDiff2_vf_pq <- boxplot.stats(data$attrakDiff2_vf_pq)$out
out_attrakDiff2_vf_hqi <- boxplot.stats(data$attrakDiff2_vf_hqi)$out
out_attrakDiff2_vf_hqs <- boxplot.stats(data$attrakDiff2_vf_hqs)$out
out_attrakDiff2_vf_hq <- boxplot.stats(data$attrakDiff2_vf_hq)$out

out_tlx_ment <- boxplot.stats(data$tlx_ment)$out
out_tlx_phys <- boxplot.stats(data$tlx_phys)$out
out_tlx_temp <- boxplot.stats(data$tlx_temp)$out
out_tlx_perf <- boxplot.stats(data$tlx_perf)$out
out_tlx_eff <- boxplot.stats(data$tlx_eff)$out
out_tlx_frust <- boxplot.stats(data$tlx_frust)$out
out_tlx_score <- boxplot.stats(data$tlx_score)$out

out_tlx_vf_ment <- boxplot.stats(data$tlx_vf_ment)$out
out_tlx_vf_phys <- boxplot.stats(data$tlx_vf_phys)$out
out_tlx_vf_temp <- boxplot.stats(data$tlx_vf_temp)$out
out_tlx_vf_perf <- boxplot.stats(data$tlx_vf_perf)$out
out_tlx_vf_eff <- boxplot.stats(data$tlx_vf_eff)$out
out_tlx_vf_frust <- boxplot.stats(data$tlx_vf_frust)$out
out_tlx_vf_score <- boxplot.stats(data$tlx_vf_score)$out

out_sart_s <- boxplot.stats(data$sart_s)$out
out_sart_d <- boxplot.stats(data$sart_d)$out
out_sart_u <- boxplot.stats(data$sart_u)$out
out_sart_sa <- boxplot.stats(data$sart_sa)$out

out_sart_vf_s <- boxplot.stats(data$sart_vf_s)$out
out_sart_vf_d <- boxplot.stats(data$sart_vf_d)$out
out_sart_vf_u <- boxplot.stats(data$sart_vf_u)$out
out_sart_vf_sa <- boxplot.stats(data$sart_vf_sa)$out
print("...done")

#------------------Check normality on raw data---------------------
print("------------------Check normality on raw data---------------------")
print("Shapiro-Wilk-sus_pq:")
print(shapiro.test(data$sus_pq))
print("sus_pq_vf")
print(shapiro.test(data$sus_pq_vf))

print("sus"               )
print(shapiro.test(data$sus))
print("sus_vf"             )
print(shapiro.test(data$sus_vf))

print("attrakDiff2_att"    )
print(shapiro.test(data$attrakDiff2_att))
print("attrakDiff2_pq"    )
print(shapiro.test(data$attrakDiff2_pq))
print("attrakDiff2_hqi"    )
print(shapiro.test(data$attrakDiff2_hqi))
print("attrakDiff2_hqs"    )
print(shapiro.test(data$attrakDiff2_hqs))
print("attrakDiff2_hq"    )
print(shapiro.test(data$attrakDiff2_hq))
print("attrakDiff2_vf_att" )
print(shapiro.test(data$attrakDiff2_vf_att))
print("attrakDiff2_vf_pq"  )
print(shapiro.test(data$attrakDiff2_vf_pq))	# no normality -> Wilcoxon-Signed-Rank-Test
print("attrakDiff2_vf_hqi")
print(shapiro.test(data$attrakDiff2_vf_hqi))
print("attrakDiff2_vf_hqs" )
print(shapiro.test(data$attrakDiff2_vf_hqs))
print("attrakDiff2_vf_hq"  )
print(shapiro.test(data$attrakDiff2_vf_hq))

print("tlx_ment"          )
print(shapiro.test(data$tlx_ment))
print("tlx_phys"           )
print(shapiro.test(data$tlx_phys))
print("tlx_temp"           )
print(shapiro.test(data$tlx_temp))
print("tlx_perf"          )
print(shapiro.test(data$tlx_perf))
print("tlx_eff"            )
print(shapiro.test(data$tlx_eff))
print("tlx_frust"          )
print(shapiro.test(data$tlx_frust))		# no normality -> Wilcoxon-Signed-Rank-Test
print("tlx_score"         )
print(shapiro.test(data$tlx_score))
print("tlx_vf_ment"        )
print(shapiro.test(data$tlx_vf_ment))
print("tlx_vf_phys"        )
print(shapiro.test(data$tlx_vf_phys))
print("tlx_vf_temp"       )
print(shapiro.test(data$tlx_vf_temp))
print("tlx_vf_perf"        )
print(shapiro.test(data$tlx_vf_perf))
print("tlx_vf_eff"         )
print(shapiro.test(data$tlx_vf_eff))	# no normality -> Wilcoxon-Signed-Rank-Test
print("tlx_vf_frust"      )
print(shapiro.test(data$tlx_vf_frust))
print("tlx_vf_score"       )
print(shapiro.test(data$tlx_vf_score))

print("sart_s"             )
print(shapiro.test(data$sart_s))
print("sart_d"            )
print(shapiro.test(data$sart_d))
print("sart_u"             )
print(shapiro.test(data$sart_u))
print("sart_sa"            )
print(shapiro.test(data$sart_sa))
print("sart_vf_s"         )
print(shapiro.test(data$sart_vf_s))
print("sart_vf_d"          )
print(shapiro.test(data$sart_vf_d))		# no normality -> Wilcoxon-Signed-Rank-Test
print("sart_vf_u"          )
print(shapiro.test(data$sart_vf_u))
print("sart_vf_sa")
print(shapiro.test(data$sart_vf_sa))
print("...done")


#------------------------Check significance/p-values on raw data---------------------------
print("------------------------Check significance/p-values on raw data---------------------------")
print("sus_pq")
print(t.test(data$sus_pq, data$sus_pq_vf, paired=TRUE))

print("sus")
print(t.test(data$sus, data$sus_vf, paired=TRUE))

print("attrakDiff2_att")
print(t.test(data$attrakDiff2_att, data$attrakDiff2_vf_att, paired=TRUE))
print("attrakDiff2_pq")
print(wilcox.test(data$attrakDiff2_pq, data$attrakDiff2_vf_pq, paired=TRUE))
print("attrakDiff2_hqi")
print(t.test(data$attrakDiff2_hqi, data$attrakDiff2_vf_hqi, paired=TRUE))
print("attrakDiff2_hqs")
print(t.test(data$attrakDiff2_hqs, data$attrakDiff2_vf_hqs, paired=TRUE))
print("attrakDiff2_hq")
print(t.test(data$attrakDiff2_hq, data$attrakDiff2_vf_hq, paired=TRUE))

print("tlx_ment")
print(t.test(data$tlx_ment, data$tlx_vf_ment, paired=TRUE))
print("tlx_ment")
print(t.test(data$tlx_ment, data$tlx_vf_ment, paired=TRUE))
print("tlx_phys")
print(t.test(data$tlx_phys, data$tlx_vf_phys, paired=TRUE))
print("tlx_temp")
print(t.test(data$tlx_temp, data$tlx_vf_temp, paired=TRUE))
print("tlx_perf")
print(t.test(data$tlx_perf, data$tlx_vf_perf, paired=TRUE))
print("tlx_eff")
print(wilcox.test(data$tlx_eff, data$tlx_vf_eff, paired=TRUE))
print("tlx_ment")
print(wilcox.test(data$tlx_frust, data$tlx_vf_frust, paired=TRUE))
print("tlx_score")
print(t.test(data$tlx_score, data$tlx_vf_score, paired=TRUE))

print("sart_s")
print(t.test(data$sart_s, data$sart_vf_s, paired=TRUE))
print("sart_d")
print(wilcox.test(data$sart_d, data$sart_vf_d, paired=TRUE))
print("sart_u")
print(t.test(data$sart_u, data$sart_vf_u, paired=TRUE))
print("sart_sa")
print(t.test(data$sart_sa, data$sart_vf_sa, paired=TRUE))
print("...done")


print("------------------Check Baysian Factor of means---------------------")
print("sus_pq")
print(ttestBF(x=data$sus_pq, y=data$sus_pq_vf, paired=TRUE))

print("sus")
print(ttestBF(x=data$sus, data$sus_vf, paired=TRUE))

print("attrakDiff2_att")
print(ttestBF(x=data$attrakDiff2_att, data$attrakDiff2_vf_att, paired=TRUE))
print("attrakDiff2_pq")
print(ttestBF(x=data$attrakDiff2_pq, data$attrakDiff2_vf_pq, paired=TRUE))
print("attrakDiff2_hqi")
print(ttestBF(x=data$attrakDiff2_hqi, data$attrakDiff2_vf_hqi, paired=TRUE))
print("attrakDiff2_hqs")
print(ttestBF(x=data$attrakDiff2_hqs, data$attrakDiff2_vf_hqs, paired=TRUE))
print("attrakDiff2_hq")
print(ttestBF(x=data$attrakDiff2_hq, data$attrakDiff2_vf_hq, paired=TRUE))

print("tlx_ment")
print(ttestBF(x=data$tlx_ment, data$tlx_vf_ment, paired=TRUE))
print("tlx_ment")
print(ttestBF(x=data$tlx_ment, data$tlx_vf_ment, paired=TRUE))
print("tlx_phys")
print(ttestBF(x=data$tlx_phys, data$tlx_vf_phys, paired=TRUE))
print("tlx_temp")
print(ttestBF(x=data$tlx_temp, data$tlx_vf_temp, paired=TRUE))
print("tlx_perf")
print(ttestBF(x=data$tlx_perf, data$tlx_vf_perf, paired=TRUE))
print("tlx_eff")
print(ttestBF(x=data$tlx_eff, data$tlx_vf_eff, paired=TRUE))
print("tlx_ment")
print(ttestBF(x=data$tlx_frust, data$tlx_vf_frust, paired=TRUE))
print("tlx_score")
print(ttestBF(x=data$tlx_score, data$tlx_vf_score, paired=TRUE))

print("sart_s")
print(ttestBF(x=data$sart_s, data$sart_vf_s, paired=TRUE))
print("sart_d")
print(ttestBF(x=data$sart_d, data$sart_vf_d, paired=TRUE))
print("sart_u")
print(ttestBF(x=data$sart_u, data$sart_vf_u, paired=TRUE))
print("sart_sa")
print(ttestBF(x=data$sart_sa, data$sart_vf_sa, paired=TRUE))
print("...done")


pdf("sus_pq_raw.pdf", width=wdth, height=hght)
boxplot(data$sus_pq, data$sus_pq_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Presence score', main='SUS-Simple')
dev.off()

pdf("sus_raw.pdf", width=wdth, height=hght)
boxplot(data$sus, data$sus_vf, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='System usability score', main='SUS')
dev.off()

pdf("tlx_score_raw.pdf", width=wdth, height=hght)
boxplot(data$tlx_score, data$tlx_vf_score, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Taskload score', main='NASA TLX')
dev.off()

pdf("sart_sa_raw.pdf", width=wdth, height=hght)
boxplot(data$sart_sa, data$sart_vf_sa, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Situation awareness score', main='SART')
dev.off()

#---------------Remove outliers-------------
print("------------------Removing outliers---------------------")
sus_pq_clean <- data$sus_pq[!data$sus_pq %in% out_sus_pq]
sus_pq_vf_clean <- data$sus_pq_vf[!data$sus_pq_vf %in% out_sus_pq_vf]

sus_clean <- data$sus[!data$sus %in% out_sus]
sus_vf_clean <- data$sus_vf[!data$sus_vf %in% out_sus_vf]

attrakDiff2_att_clean <- data$attrakDiff2_att[!data$attrakDiff2_att %in% out_attrakDiff2_att]
attrakDiff2_pq_clean <- data$attrakDiff2_pq[!data$attrakDiff2_pq %in% out_attrakDiff2_pq]
attrakDiff2_hqi_clean <- data$attrakDiff2_hqi[!data$attrakDiff2_hqi %in% out_attrakDiff2_hqi]
attrakDiff2_hqs_clean <- data$attrakDiff2_hqs[!data$attrakDiff2_hqs %in% out_attrakDiff2_hqs]
attrakDiff2_hq_clean <- data$attrakDiff2_hq[!data$attrakDiff2_hq %in% out_attrakDiff2_hq]

attrakDiff2_vf_att_clean <- data$attrakDiff2_vf_att[!data$attrakDiff2_vf_att %in% out_attrakDiff2_vf_att]
attrakDiff2_vf_pq_clean <- data$attrakDiff2_vf_pq[!data$attrakDiff2_vf_pq %in% out_attrakDiff2_vf_pq]
attrakDiff2_vf_hqi_clean <- data$attrakDiff2_vf_hqi[!data$attrakDiff2_vf_hqi %in% out_attrakDiff2_vf_hqi]
attrakDiff2_vf_hqs_clean <- data$attrakDiff2_vf_hqs[!data$attrakDiff2_vf_hqs %in% out_attrakDiff2_vf_hqs]
attrakDiff2_vf_hq_clean <- data$attrakDiff2_vf_hq[!data$attrakDiff2_vf_hq %in% out_attrakDiff2_vf_hq]

tlx_ment_clean <- data$tlx_ment[!data$tlx_ment %in% out_tlx_ment]
tlx_phys_clean <- data$tlx_phys[!data$tlx_phys %in% out_tlx_phys]
tlx_temp_clean <- data$tlx_temp[!data$tlx_temp %in% out_tlx_temp]
tlx_perf_clean <- data$tlx_perf[!data$tlx_perf %in% out_tlx_perf]
tlx_eff_clean <- data$tlx_eff[!data$tlx_eff %in% out_tlx_eff]
tlx_frust_clean <- data$tlx_frust[!data$tlx_frust %in% out_tlx_frust]
tlx_score_clean <- data$tlx_score[!data$tlx_score %in% out_tlx_score]

tlx_vf_ment_clean <- data$tlx_vf_ment[!data$tlx_vf_ment %in% out_tlx_vf_ment]
tlx_vf_phys_clean <- data$tlx_vf_phys[!data$tlx_vf_phys %in% out_tlx_vf_phys]
tlx_vf_temp_clean <- data$tlx_vf_temp[!data$tlx_vf_temp %in% out_tlx_vf_temp]
tlx_vf_perf_clean <- data$tlx_vf_perf[!data$tlx_vf_perf %in% out_tlx_vf_perf]
tlx_vf_eff_clean <- data$tlx_vf_eff[!data$tlx_vf_eff %in% out_tlx_vf_eff]
tlx_vf_frust_clean <- data$tlx_vf_frust[!data$tlx_vf_frust %in% out_tlx_vf_frust]
tlx_vf_score_clean <- data$tlx_vf_score[!data$tlx_vf_score %in% out_tlx_vf_score]

sart_s_clean <- data$sart_s[!data$sart_s %in% out_sart_s]
sart_d_clean <- data$sart_d[!data$sart_d %in% out_sart_d]
sart_u_clean <- data$sart_u[!data$sart_u %in% out_sart_u]
sart_sa_clean <- data$sart_sa[!data$sart_sa %in% out_sart_sa]

sart_vf_s_clean <- data$sart_vf_s[!data$sart_vf_s %in% out_sart_vf_s]
sart_vf_d_clean <- data$sart_vf_d[!data$sart_vf_d %in% out_sart_vf_d]
sart_vf_u_clean <- data$sart_vf_u[!data$sart_vf_u %in% out_sart_vf_u]
sart_vf_sa_clean <- data$sart_vf_sa[!data$sart_vf_sa %in% out_sart_vf_sa]
print("...done")


#------------------Check normality on data without outliers---------------------
print("------------------Check normality on data without outliers---------------------")
print("Shapiro-Wilk-sus_pq:")
print(shapiro.test(sus_pq_clean))
print("sus_pq_vf")
print(shapiro.test(sus_pq_vf_clean))

print("sus"               )
print(shapiro.test(sus_clean))
print("sus_vf"             )
print(shapiro.test(sus_vf_clean))

print("attrakDiff2_att"    )
print(shapiro.test(attrakDiff2_att_clean))
print("attrakDiff2_pq"    )
print(shapiro.test(attrakDiff2_pq_clean))
print("attrakDiff2_hqi"    )
print(shapiro.test(attrakDiff2_hqi_clean))
print("attrakDiff2_hqs"    )
print(shapiro.test(attrakDiff2_hqs_clean))		# no normality
print("attrakDiff2_hq"    )
print(shapiro.test(attrakDiff2_hq_clean))
print("attrakDiff2_vf_att" )
print(shapiro.test(attrakDiff2_vf_att_clean))
print("attrakDiff2_vf_pq"  )
print(shapiro.test(attrakDiff2_vf_pq_clean))	
print("attrakDiff2_vf_hqi")
print(shapiro.test(attrakDiff2_vf_hqi_clean))
print("attrakDiff2_vf_hqs" )
print(shapiro.test(attrakDiff2_vf_hqs_clean))  # no normality
print("attrakDiff2_vf_hq"  )
print(shapiro.test(attrakDiff2_vf_hq_clean))

print("tlx_ment"          )
print(shapiro.test(tlx_ment_clean))
print("tlx_phys"           )
print(shapiro.test(tlx_phys_clean))
print("tlx_temp"           )
print(shapiro.test(tlx_temp_clean))
print("tlx_perf"          )
print(shapiro.test(tlx_perf_clean))
print("tlx_eff"            )
print(shapiro.test(tlx_eff_clean))
print("tlx_frust"          )
print(shapiro.test(tlx_frust_clean))		# no normality -> Wilcoxon-Signed-Rank-Test
print("tlx_score"         )
print(shapiro.test(tlx_score_clean))
print("tlx_vf_ment"        )
print(shapiro.test(tlx_vf_ment_clean))
print("tlx_vf_phys"        )
print(shapiro.test(tlx_vf_phys_clean))
print("tlx_vf_temp"       )
print(shapiro.test(tlx_vf_temp_clean))
print("tlx_vf_perf"        )
print(shapiro.test(tlx_vf_perf_clean))
print("tlx_vf_eff"         )
print(shapiro.test(tlx_vf_eff_clean))	# no normality -> Wilcoxon-Signed-Rank-Test
print("tlx_vf_frust"      )
print(shapiro.test(tlx_vf_frust_clean))
print("tlx_vf_score"       )
print(shapiro.test(tlx_vf_score_clean))

print("sart_s"             )
print(shapiro.test(sart_s_clean))
print("sart_d"            )
print(shapiro.test(sart_d_clean))
print("sart_u"             )
print(shapiro.test(sart_u_clean))
print("sart_sa"            )
print(shapiro.test(sart_sa_clean))
print("sart_vf_s"         )
print(shapiro.test(sart_vf_s_clean))
print("sart_vf_d"          )
print(shapiro.test(sart_vf_d_clean))		# no normality -> Wilcoxon-Signed-Rank-Test
print("sart_vf_u"          )
print(shapiro.test(sart_vf_u_clean))
print("sart_vf_sa")
print(shapiro.test(sart_vf_sa_clean))
print("...done")



#------------------------Check significance/p-values on raw data---------------------------
print("------------------------Check significance/p-values on clean data---------------------------")
print("sus_pq")
print(t.test(sus_pq_clean, sus_pq_vf_clean))

print("sus")
print(t.test(sus_clean, sus_vf_clean))

print("attrakDiff2_att")
print(t.test(attrakDiff2_att_clean, attrakDiff2_vf_att_clean))
print("attrakDiff2_pq")
print(t.test(attrakDiff2_pq_clean, attrakDiff2_vf_pq_clean))
print("attrakDiff2_hqi")
print(t.test(attrakDiff2_hqi_clean, attrakDiff2_vf_hqi_clean))
print("attrakDiff2_hqs")
print(wilcox.test(attrakDiff2_hqs_clean, attrakDiff2_vf_hqs_clean))
print("attrakDiff2_hq")
print(t.test(attrakDiff2_hq_clean, attrakDiff2_vf_hq_clean))

print("tlx_ment")
print(t.test(tlx_ment_clean, tlx_vf_ment_clean))
print("tlx_ment")
print(t.test(tlx_ment_clean, tlx_vf_ment_clean))
print("tlx_phys")
print(t.test(tlx_phys_clean, tlx_vf_phys_clean))
print("tlx_temp")
print(t.test(tlx_temp_clean, tlx_vf_temp_clean))
print("tlx_perf")
print(t.test(tlx_perf_clean, tlx_vf_perf_clean))
print("tlx_eff")
print(wilcox.test(tlx_eff_clean, tlx_vf_eff_clean))
print("tlx_ment")
print(wilcox.test(tlx_frust_clean, tlx_vf_frust_clean))
print("tlx_score")
print(t.test(tlx_score_clean, tlx_vf_score_clean))

print("sart_s")
print(t.test(sart_s_clean, sart_vf_s_clean))
print("sart_d")
print(wilcox.test(sart_d_clean, sart_vf_d_clean))
print("sart_u")
print(t.test(sart_u_clean, sart_vf_u_clean))
print("sart_sa")
print(t.test(sart_sa_clean, sart_vf_sa_clean))
print("...done")

# close the output file
sink()



pdf("sus_pq.pdf", width=wdth, height=hght)
boxplot(sus_pq_clean, sus_pq_vf_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Presence score', main='SUS-PQ')
dev.off()

pdf("sus.pdf", width=wdth, height=hght)
boxplot(sus_clean, sus_vf_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='System usability score', main='SUS')
dev.off()

pdf("tlx_score.pdf", width=wdth, height=hght)
boxplot(tlx_score_clean, tlx_vf_score_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Taskload score', main='NASA TLX')
dev.off()

pdf("sart_sa.pdf", width=wdth, height=hght)
boxplot(sart_sa_clean, sart_vf_sa_clean, names=c('without VF','VF'), col=c(colNoVF,colVF), ylab='Situation awareness score', main='SART')
dev.off()

# unload the libraries
#detach("package:rgl")

# change back to the original directory
#setwd(initial.dir)