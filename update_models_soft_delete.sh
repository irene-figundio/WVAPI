#!/bin/bash
find DataAccess/Database/Models -name "*.cs" | while read -r file; do
    if grep -q "class " "$file"; then
        if ! grep -q "public bool? IsDeleted" "$file"; then
            sed -i '/public class/!b;n;a\        public bool? IsDeleted { get; set; }\n        public DateTime? DeletionDate { get; set; }' "$file"
            echo "Updated $file"
        fi
    fi
done
