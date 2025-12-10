And here's the interface. Pretty straightforward.

This is your field. You drag plants from the library below and place them. 

Let's start simple—an oak tree. # event Planting

Just drag it up. # wait_event drag

// [玩家拖拽 - 自动等待]

There. The system handles the rest. # stop_highlights

// [暂停，让玩家看到树开始生长]

See it growing? That indicator tracks its progress. # event indicator

// [等一会儿]

Plants survive by consuming fertility from the soil. # event fertility_bar

That's the bar on the left. The soil's energy reserve.

They also need sunlight. Notice the shade under the tree? # event highlight_tree_shade

Just like reality—shade changes the light environment beneath it.


Some species prefer shade. Try placing this mycelium under the tree. # event highlight_mushroom_card # wait_event plant_mushroom

// [玩家种蘑菇 - 自动等待]

If conditions are right, plants thrive. # stop_highlights

If not... well, you'll see.

// [如果玩家失败]

Don't worry. Trial and error. That's how ecosystems work.