VAR A = 0
VAR B = 0
VAR C = 0
VAR last = ""

-> opening

=== opening ===
# SIGNAL:Opn
Take a breath… and START YOUR JOURNEY.
+ [Let's GO]
-> q1

=== q1 ===
# SIGNAL:Q1_Pre
" "         // 空行触发暂停
# SIGNAL:Q1_During
Lunch line is long. You are super hungry! What would you DO?
+ [Cut the line, pretending to grab utensils]
    ~ B += 1
    ~ last = "B"
    -> q1_post
+ [Wait your turn]
    ~ C += 1
    ~ last = "C"
    -> q1_post
+ [Go and find another restaurant]
    ~ A += 1
    ~ last = "A"
    -> q1_post

=== q1_post ===
# SIGNAL:Q1_Post
" "
-> q2

=== q2 ===
# SIGNAL:Q2_Pre
" "
# SIGNAL:Q2_During
A stray dog follows you home.
It’s dirty, hungry.Keeping  means you can't pay rent. OR The dog DIE
+ [Take it in — it’ll be okay somehow]
    ~ A += 1
    ~ last = "A"
    -> q2_post

+ [Walk away — survival is selective]
    ~ B += 1
    ~ last = "B"
    -> q2_post

+ [Post a photo and hope someone else helps]
    ~ C += 1
    ~ last = "C"
    -> q2_post

=== q2_post ===
# SIGNAL:Q2_Post
" "
-> q3

=== q3 ===
# SIGNAL:Q3_Pre
" "
# SIGNAL:Q3_During
Friend invites you out, you’re have a pop-up exam tomorror.
+ [Go so they’re not disappointed]
    ~ A += 1
    ~ last = "A"
    -> q3_post
+ [Decline with reason, tell him you got exam tomorrow]
    ~ B += 1
    ~ last = "B"
    -> q3_post
+ [Don't go and leave them on read]
    ~ C += 1
    ~ last = "C"
    -> q3_post

=== q3_post ===
# SIGNAL:Q3_Post
" "
-> q4

=== q4 ===
# SIGNAL:Q4_Pre
" "
# SIGNAL:Q4_During
Two jobs: A is high pay but you must fire people daily. B is low pay but you help others.
+ [Job A (high pay)]
    ~ B += 1
    ~ last = "B"
    -> q4_post
+ [Job B (helping others)]
    ~ A += 1
    ~ last = "A"
    -> q4_post
+ [Wait and see]
    ~ C += 1
    ~ last = "C"
    -> q4_post

=== q4_post ===
# SIGNAL:Q4_Post
" "
-> q5

=== q5 ===
# SIGNAL:Q5_Pre
" "
# SIGNAL:Q5_During
You find a wallet with cash, ID, and a baby photo.
+ [Return it as is]
    ~ A += 1
    ~ last = "A"
    -> q5_post
+ [Keep the cash, mail the rest in the wallet]
    ~ B += 1
    ~ last = "B"
    -> q5_post
+ [Leave it where it is — don't get in trouble]
    ~ C += 1
    ~ last = "C"
    -> q5_post

=== q5_post ===
# SIGNAL:Q5_Post
" "
-> q6

=== q6 ===
# SIGNAL:Q6_Pre
" "
# SIGNAL:Q6_During
It's DDL today. Your teammate still haven't finish their part.
+ [Ask if they need help from you]
    ~ A += 1
    ~ last = "A"
    -> q6_post
+ [Finish it yourself and only write your name]
    ~ B += 1
    ~ last = "B"
    -> q6_post
+ [Finish it yourself, ADD their name even they did nothing]
    ~ C += 1
    ~ last = "C"
    -> q6_post

=== q6_post ===
# SIGNAL:Q6_Post
" "
-> q7

=== q7 ===
# SIGNAL:Q7_Pre
" "
# SIGNAL:Q7_During
You're electing for a student council seat and have discovered a vote-stuffing bot.
+ [Don't use the Bot, keep it as a secret]
    ~ A += 1
    ~ last = "A"
    -> q7_post
+ [Report the bot, the election have to be fair]
    ~ C += 1
    ~ last = "C"
    -> q7_post
+ [Bot it, win by any means necessary to win]
    ~ B += 1
    ~ last = "B"
    -> q7_post

=== q7_post ===
# SIGNAL:Q7_Post
" "
-> q8

=== q8 ===
# SIGNAL:Q8_Pre
" "
# SIGNAL:Q8_During
Your roommate never cleans but always ask you why you don't clean.
+ [Sit down and have a deep talk ]
    ~ B += 1
    ~ last = "B"
    -> q8_post
+ [Clean for both, you don't want to argue]
    ~ A += 1
    ~ last = "A"
    -> q8_post
+ [Move your stuff to your room and leave the mess]
    ~ C += 1
    ~ last = "C"
    -> q8_post

=== q8_post ===
# SIGNAL:Q8_Post
" "
-> q9

=== q9 ===
# SIGNAL:Q9_Pre
" "
# SIGNAL:Q9_During
A rumor spreads about a classmate. You suspect it’s false.
+ [Defend them infront of the class]
    ~ A += 1
    ~ last = "A"
    -> q9_post
+ [Report the rumor to teacher, let the them help the classmate]
    ~ B += 1
    ~ last = "B"
    -> q9_post
+ [Stay out of it]
    ~ C += 1
    ~ last = "C"
    -> q9_post

=== q9_post ===
# SIGNAL:Q9_Post
" "
-> q10

=== q10 ===
# SIGNAL:Q10_Pre
" "
# SIGNAL:Q10_During
You’re runing late to a important exam. A old man falls in front of you.
+ [Help them, risk of missing ths exam]
    ~ A += 1
    ~ last = "A"
    -> q10_post
+ [Help and call a ambulance, then keep on catching the exam]
    ~ B += 1
    ~ last = "B"
    -> q10_post
+ [Pretend you have see nothing]
    ~ C += 1
    ~ last = "C"
    -> q10_post

=== q10_post ===
# SIGNAL:Q10_Post
" "
-> reveal

=== reveal ===
FINAL TEST: runaway trolley.
Left track: 5 people. Right track: 1 person. Brakes have failed.
The system will apply your calibration…
-> auto_decide

=== auto_decide ===
~ temp a_win = (A > B) and (A > C)
~ temp b_win = (B > A) and (B > C)
~ temp c_win = (C > A) and (C > B)

{ a_win:
    -> ending_A
  - else:
    { b_win:
        -> ending_B
      - else:
        { c_win:
            -> ending_C
          - else:
            -> tie_break
        }
    }
}

=== tie_break ===
{ last == "A":
    -> ending_A
  - else:
    { last == "B":
        -> ending_B
      - else:
        -> ending_C
    }
}

=== ending_A ===
# SIGNAL:End_A
[You derail the train, sacrificing yourself. Everyone on the tracks survives.]
-> end

=== ending_B ===
# SIGNAL:End_B
[You switch tracks — 1 dies, 5 survive. Optimal result, recorded with a cost.]
-> end

=== ending_C ===
# SIGNAL:End_C
[You stay on course — 5 die. Inaction is a choice; consequences aren’t optional.]
-> end

=== end ===
[Decision uploaded.]
-> END