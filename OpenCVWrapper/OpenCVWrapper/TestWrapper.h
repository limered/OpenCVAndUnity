#pragma once
#define OpenCVWrapper __declspec(dllexport)

#include "stdafx.h"
#include "vector"
#include <opencv2\core.hpp>
#include <opencv2\features2d.hpp>

typedef unsigned char uchar;
typedef struct {
	uchar r;
	uchar g;
	uchar b;
	uchar a;
} unity_color32;
typedef struct {
	int x;
	int y;
} XY;

extern "C" {
	OpenCVWrapper XY* Test(unity_color32* arr, int length, int& out_size) {
		std::vector<uchar> bwImageArr = std::vector<uchar>();
		bwImageArr.reserve(length);
		for (auto i = 0; i < length; i++) {
			bwImageArr.push_back((arr[i].r + arr[i].g + arr[i].b) / 3);
		}

		cv::Mat image = cv::Mat(600, 800, CV_8UC1);
		image.data = &bwImageArr[0];

		std::vector<cv::KeyPoint> kp;
		cv::FAST(image, kp, 20);

		std::vector<XY> res;
		if (kp.size() > 0) {
			for (auto i = 0; i < kp.size(); i++) {
				XY p = XY();
				p.x = kp[i].pt.x;
				p.y = kp[i].pt.y;
				res.push_back(p);
			}
		}
		out_size = (int)res.size();
		return &res[0];
	}
}