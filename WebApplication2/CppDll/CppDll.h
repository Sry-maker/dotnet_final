#pragma once
#include <stdio.h>
#include <stdlib.h>

#ifdef CPPDLL_EXPORTS
#define CPP_EXPORTS __declspec(dllexport)
#else
#define CPP_EXPORTS __declspec(dllimport)
#endif

extern "C" CPP_EXPORTS double Add(double a, double b);

extern "C" CPP_EXPORTS double Sub(double a, double b);

extern "C" CPP_EXPORTS double Mul(double a, double b);

extern "C" CPP_EXPORTS  double Div(double a, double b);
