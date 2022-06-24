// CliDll.h
#pragma once
#include <iostream>
#include "CppDll.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;
using namespace System::Collections;
using namespace std;

#pragma comment(lib, "CppDll.lib")
#pragma managed
namespace CliDll {

	public ref class Arith
	{
	public:
		Arith();
		~Arith();

		double AddCli(double a, double b);
		double SubCli(double a, double b);
		double MulCli(double a, double b);
		double DivCli(double a, double b);
	};
}