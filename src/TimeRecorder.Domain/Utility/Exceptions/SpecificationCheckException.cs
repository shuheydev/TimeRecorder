﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeRecorder.Domain.Utility.Exceptions
{
    public class SpecificationCheckException : Exception
    {
        public SpecificationCheckException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public ValidationResult ValidationResult { get; }
    }
}