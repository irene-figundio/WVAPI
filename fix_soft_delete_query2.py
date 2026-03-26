import os
import glob
import re

def update_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Need to clean up the bad sed changes first if they exist
    content = re.sub(r'\.Query<([a-zA-Z0-9_]+)>\.Where\(e =>\.Where\(e => e\.IsDeleted != true\) e\.IsDeleted != true\)\(\)', r'.Query<\1>()', content)
    content = re.sub(r'\.Query<([a-zA-Z0-9_]+)>\.Where\(e => e\.IsDeleted != true\)\(\)', r'.Query<\1>()', content)
    content = re.sub(r'\.Query<([a-zA-Z0-9_]+)>\(.*\)', r'.Query<\1>()', content)

    # Now apply
    content = re.sub(r'\.Query<([a-zA-Z0-9_]+)>\(\)', r'.Query<\1>().Where(e => e.IsDeleted != true)', content)

    # Replace _unitOfWork.Remove(x);
    content = re.sub(r'_unitOfWork\.Remove\(([^)]+)\);', r'\1.IsDeleted = true;\n                \1.DeletionDate = DateTime.Now;\n                _unitOfWork.Update(\1);', content)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

for filepath in glob.glob('Controllers/*.cs'):
    update_file(filepath)
