import React from 'react';
import RatingStars from './RatingStars';

function CustomComment({index, comment }) {

  return (
    <div key={index} className="d-flex flex-row comment-row">
      <div className="p-2">
        <span className="round">
          <img src="https://iconape.com/wp-content/png_logo_vector/avatar-11.png" alt="user" width="50" />
        </span>
      </div>
      <div className="comment-text w-100">
        <h5>{comment.userFullName}</h5>
        <div className="comment-footer">
          <span className="date">{comment.commentDateISOtype}</span>
          <span className={`label label-${comment.status}`} style={{ marginLeft: '10px' }}>{comment.labelText}</span>
          <div class="d-flex justify-content-between align-items-center">
            <div class="ratings">

              <RatingStars rate={comment.rate} />

            </div>
            <h5 className="review-count">{comment.reviewLikeCount} Beğenme</h5>
          </div>
        </div>
        <p className="m-b-5 m-t-10">{comment.comment}</p>
      </div>
    </div>
  );
}

export default CustomComment;
