using System;
using System.Collections.Generic;

namespace server.Dto;

public record DropdownOptionDto(Guid Id, string Label);

public record AttributeCategoryDto(int Id, string Name);
