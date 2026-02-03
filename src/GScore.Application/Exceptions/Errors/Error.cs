using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Exceptions.Errors;

public sealed record Error(string Code, string Message);