// CliDll.cpp
#include "CliDll.h"

using namespace CliDll;

CliDll::Arith::Arith() {}

CliDll::Arith::~Arith() {}

double CliDll::Arith::AddCli(double a, double b)
{
	return Add(a, b);
}

double CliDll::Arith::SubCli(double a, double b)
{
	return Sub(a, b);
}

double CliDll::Arith::MulCli(double a, double b)
{
	return Mul(a, b);
}

double CliDll::Arith::DivCli(double a, double b)
{
	return Div(a, b);
}