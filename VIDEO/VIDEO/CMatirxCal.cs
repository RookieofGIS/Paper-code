using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFenceSys.VideoMap
{
    class CMatirxCal
    {
        public  static double[,] MatInver(double[,] n) //矩阵求逆函数  元组法改进  
        {
            //前提判断： 是否为方阵  是否可逆
            int m = n.GetLength(0);
            double[,] q = new double[m, m]; //求逆结果
            int i, j, k;//计数君
            double u, temp;//临时变量

            //初始单位阵
            for (i = 0; i < m; i++)
                for (j = 0; j <= m - 1; j++)
                    q[i, j] = (i == j) ? 1 : 0;

            /// 求左下
            ///
            for (i = 0; i <= m - 2; i++)
            {
                //提取该行的主对角线元素
                u = n[i, i];   //可能为0
                if (u == 0)  //为0 时，在下方搜索一行不为0的行并交换
                {
                    for (i = 0; i < m; i++)
                    {
                        k = i;
                        for (j = i + 1; j < m; j++)
                        {
                            if (n[j, i] != 0) //不为0的元素
                            {
                                k = j;
                                break;
                            }
                        }

                        if (k != i) //如果没有发生交换： 情况1 下方元素也全是0
                        {
                            for (j = 0; j < m; j++)
                            {
                                //行交换
                                temp = n[i, j];
                                n[i, j] = n[k, j];
                                n[k, j] = temp;
                                //伴随交换
                                temp = q[i, j];
                                q[i, j] = q[k, j];
                                q[k, j] = temp;
                            }
                        }
                        else //满足条件1 弹窗提示
                            ;
                            //MessageBox.Show("不可逆矩阵", "ERROR", MessageBoxButtons.OK);

                    }
                }

                for (j = 0; j < m; j++)//该行除以主对角线元素的值 使主对角线元素为1  
                {
                    n[i, j] = n[i, j] / u;   //分母不为0
                    q[i, j] = q[i, j] / u;  //伴随矩阵
                }

                for (k = i + 1; k < m; k++)  //下方的每一行减去  该行的倍数
                {
                    u = n[k, i];   //下方的某一行的主对角线元素
                    for (j = 0; j < m; j++)
                    {
                        n[k, j] = n[k, j] - u * n[i, j];  //下方的每一行减去该行的倍数  使左下角矩阵化为0
                        q[k, j] = q[k, j] - u * q[i, j];  //左下伴随矩阵
                    }
                }
            }


            u = n[m - 1, m - 1];  //最后一行最后一个元素

            if (u == 0) //条件2 初步计算后最后一行全是0 在只上步骤中没有计算最后一行，所以可能会遗漏
                ;
                //MessageBox.Show("不可逆矩阵", "ERROR", MessageBoxButtons.OK);
            n[m - 1, m - 1] = 1;
            for (j = 0; j < m; j++)
            {
                q[m - 1, j] = q[m - 1, j] / u;
            }

            // 求右上
            for (i = m - 1; i >= 0; i--)
            {
                for (k = i - 1; k >= 0; k--)
                {
                    u = n[k, i];
                    for (j = 0; j < m; j++)
                    {
                        n[k, j] = n[k, j] - u * n[i, j];
                        q[k, j] = q[k, j] - u * q[i, j];
                    }
                }
            }
            return q;
        }
        public static double[,] GetPixCoors(double[,] geoxy,double [,] mInverst)
        {
            int nrow = geoxy.GetLength(0);
            double[,] q = new double[nrow, 2];
            for (int i = 0; i < nrow; i++)
            {

                double geox = geoxy[i,0];
                double geoy = geoxy[i,1];
                double Xpic = mInverst[0, 0] * geox + mInverst[0, 1] * geoy + mInverst[0, 2] * 1;
                double Ypic = mInverst[1, 0] * geox + mInverst[1, 1] * geoy + mInverst[1, 2] * 1;
                double tmp = mInverst[2, 0] * geox + mInverst[2, 1] * geoy + mInverst[2, 2] * 1;
                Xpic = Xpic / tmp;
                Ypic = Ypic / tmp;
                q[i, 0] = Xpic;
                q[i, 1] = Ypic;
               
            }
            return q;
        }
    }
}
