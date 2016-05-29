#ifndef HEADER_FIRST
#define HEADER_FIRST

#include<iostream>
using namespace std;
#define NUM 5

class First
{
public:
	void BinarySearch(int m, int a[],int len)
	{
		int low, hig, mid;
		low = 0;
		hig = len;
		mid = (low + hig) / 2;                //找到分界，中间值
		bool flag = 0;
		while (!flag)
		{
			if (a[low] == m)                  //找到
			{
				flag = 1;
				cout << m << "在这组数的第" << low + 1 << "位。" << "\n";
			}
			else if (a[hig] == m)
			{
				flag = 1;
				cout << m << "在这组数的第" << low + 1 << "位。" << "\n";
			}
			else if (low == hig || low == hig - 1 && hig != m&&low != m)
			{
				flag = 1;
				cout << "未找到数" << m << "\n";
			}
			else
			{
				mid = (low + hig) / 2;
				if (a[mid]>m)hig = mid;       //判断要查询的数与mid值的大小比较，若比中值大，查找右半边
				else low = mid;               //否则查找左半边
			}

		}

	}

    void QuickSort(int a[],int first,int last){

        if (first >= last)                        //当已经全部排序，结束
		{
			return;
		}
		int low,hig,key;                    //定义变量，low ,hig,做位标记，key为用来比较大小的键值
		hig = last;
		low = first;
		key = a[first];                        //key值随机，此处选取数组第一个数
		while (low < hig)
		{
			while (low<hig&&a[hig] >= key)     //从数组最后一个数，查找第一个小于key的数
			{
				--hig;
			}
			a[low] = a[hig];                   //找到之后把较小值放在低端
			//a[hig] = key;
			while (low<hig&&a[low] <= key){    //从前往后查找第一个大于key的值
				low++;
			}
			a[hig] = a[low];                   //把大值放到高端
			//a[low] = key;
		}
		a[low] = key;                          //一趟排序结束
		QuickSort(a, first, low-1);            //对两边分别进行快排
		QuickSort(a, hig+1, last);
	}
	
};

#endif