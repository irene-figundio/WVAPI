awk '
/Subtitle = e\.Subtitle/ {
    count_sub++
    if (count_sub > 1) { count_sub=0; next }
}
/CategoryId = e\.CategoryId/ {
    count_cat++
    if (count_cat > 1) { count_cat=0; next }
}
/Coordinates = e\.Coordinates/ {
    count_coord++
    if (count_coord > 1) { count_coord=0; next }
}
/HeroImage = e\.HeroImage/ {
    count_hero++
    if (count_hero > 1) { count_hero=0; next }
}
{ print }
' Controllers/EventsController.cs > temp.cs && mv temp.cs Controllers/EventsController.cs
