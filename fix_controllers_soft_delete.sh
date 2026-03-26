#!/bin/bash

find Controllers -name "*.cs" | while read -r file; do
    # Replace Get where it doesn't already have IsDeleted filter
    # This might need manual inspection, let's inject a basic where clause
    sed -i 's/\.Query<\(.*\)>\()\|\)/\0.Where(e => e.IsDeleted != true)/g' "$file"

    # Replace Removes with Update
    sed -i 's/_unitOfWork.Remove(\(.*\));/\1.IsDeleted = true;\n                \1.DeletionDate = DateTime.Now;\n                _unitOfWork.Update(\1);/g' "$file"

    echo "Patched SoftDelete in $file"
done
